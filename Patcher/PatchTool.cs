using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILRepacking;
using System.IO;
using PBase;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace Patcher
{
	public class PatchTool
	{
		private Logger logger = new Logger();
		private AssemblyDef Target;
		public PatchTool()
		{

		}
		private void Merge(string target, string result, string[] patches)
		{
			string[] str = new string[] { "/union", "/norepackres", "/ndebug", "/copyattrs", "/out:" + result, target }.Concat(patches).ToArray();
			ILRepack ilr = new ILRepack(new RepackOptions(str));
			ilr.Repack();
			System.Threading.Thread.Sleep(1000);
		}
		public void Patch(string target, string result, string[] patches, string[] pys)
		{

			Target = AssemblyDef.Load(target);

			string ASMName = Target.ManifestModule.Name;
			if (ASMName.EndsWith(".exe") || ASMName.EndsWith(".dll"))
			{
				ASMName = ASMName.Substring(0, ASMName.LastIndexOf('.'));
			}

			//Public
			logger.Log("Making all members public......");
			PublicAllMember();

			//Save temp file
			string tmpFile = Path.GetTempFileName();
			Save(tmpFile);

			logger.Log("Merging......");
			Merge(tmpFile, result, patches.ToArray());

			//Load the merged file
			Target = AssemblyDef.Load(result);

			//Delete temp file
			while (true)
			{
				try
				{
					File.Delete(tmpFile);
				}
				catch (UnauthorizedAccessException e)
				{
					e.GetBaseException();
					continue;
				}
				break;
			}

			logger.Log("Patching......");
			logger.Log("Target:\t\t\t\"" + Target.FullName + "\"");

			logger.Log("");


			logger.Level++;

			//
			MoveType();
			MergeType();

			logger.Log("\nProcessing methods......");
			logger.Level++;
			ProcessMethods(pys);
			logger.Level--;
			//


			logger.Level--;

			logger.Log("");
			logger.Log("Finished patching");
			Target.ManifestModule.Name = ASMName;
			Target.Name = ASMName;
			Save(result);
		}


		private void Save(string fileName)
		{
			Stream s = File.Open(fileName, FileMode.OpenOrCreate);
			Target.Write(s);
			s.Close();
			logger.Log("File saved:\t\"" + fileName + "\"");
		}

		private void ProcessMethods(string[] patches)
		{
			foreach (var patch in patches)
			{
				logger.Log("Executing script:\t\t" + patch);
				ScriptRuntime runtime = Python.CreateRuntime();
				ScriptEngine Engine = runtime.GetEngine("Python");
				var paths = Engine.GetSearchPaths();
				paths.Add(Environment.CurrentDirectory);
				Engine.SetSearchPaths(paths);
				dynamic py = Engine.ExecuteFile(patch);
				foreach (var type in Target.ManifestModule.Types)
				{
					ProcessMethods_S(py, type);
				}
				runtime.Shutdown();
			}
		}

		private void ProcessMethods_S(dynamic py, TypeDef type)
		{
			foreach (var method in type.Methods)
			{
				py.ProcessMethod(method);
			}
			type.NestedTypes.ToList().ForEach(t =>
			{
				ProcessMethods_S(py, t);
			});
		}

		private bool HasClass(string v)
		{
			foreach (var s in Target.ManifestModule.Types)
				if (s.FullName == v) return true;
			return false;
		}
		private Hashtable _mm_table = new Hashtable();
		private bool HasMethod(string v)
		{
			if (_mm_table[v] != null) return (int)_mm_table[v] == 1;
			foreach (var s in Target.ManifestModule.Types)
			{
				foreach (var m in s.Methods)
				{
					_mm_table[v] = 1;
					if (m.FullName == v) return true;
				}
			}
			_mm_table[v] = 0;
			return false;
		}

		private void MoveType()
		{
			for (int i = 0; i < Target.ManifestModule.Types.Count();)
			{
				var type = Target.ManifestModule.Types[i];
				i++;
				foreach (var typeAttr in type.CustomAttributes)
				{
					if (typeAttr.AttributeType.FullName != "PBase.PDependence") continue;
					PDependenceOption o = (PDependenceOption)typeAttr.ConstructorArguments[0].Value;
					bool isClass = (bool)typeAttr.ConstructorArguments[1].Value;
					string name = (UTF8String)typeAttr.ConstructorArguments[2].Value;
					if (isClass && !HasClass(name))
					{
						if (o == PDependenceOption.Error)
							throw new PatchException("[" + type.FullName + "]Error: Dependence named \"" + name + "\" not found");
						else if (o == PDependenceOption.Warning)
							logger.Log("[" + type.FullName + "]Warning: Dependence named \"" + name + "\" not found");
						else if (o == PDependenceOption.Skip)
						{
							logger.Log("[" + type.FullName + "]Skipped: Dependence named \"" + name + "\" not found");
							return;
						}
					}
					else if (!isClass && !HasMethod(name))
					{
						if (o == PDependenceOption.Error)
							throw new PatchException("");
						else if (o == PDependenceOption.Warning)
							logger.Log("[" + type.FullName + "]Warning: Dependence named \"" + name + "\" not found");
						else if (o == PDependenceOption.Skip)
						{
							logger.Log("[" + type.FullName + "]Skipped: Dependence named \"" + name + "\" not found");
							return;
						}
					}
				}
				CustomAttribute typeAttr1 = FirstCustomAttributes(type, "PBase.PPatch");
				if (typeAttr1 == null) continue;
				PPatchOption option = (PPatchOption)typeAttr1.ConstructorArguments[0].Value;
				if (option != PPatchOption.Move) continue;

				type.CustomAttributes.Remove(typeAttr1);

				logger.Log("[" + option + "] Type founded:\t\"" + type.FullName + "\"");
				string Namespace = (UTF8String)typeAttr1.ConstructorArguments[1].Value;
				string TypeName = (UTF8String)typeAttr1.ConstructorArguments[2].Value;
				string TypeDescription = (UTF8String)typeAttr1.ConstructorArguments[3].Value;

				logger.Level++;
				logger.Log("Moving type \"" + type.FullName + "\" to \"" + Namespace + "\"");
				var ty = Target.ManifestModule.Types.Where(t => t.Namespace == Namespace && t.Name == TypeName);
				if (ty.Count() != 0)
				{
					logger.Log("Can't move to a exists type");
					throw new PatchException("Can't move to a exists type");
				}
				Target.ManifestModule.Types.Remove(type);
				type.Namespace = Namespace;
				type.Name = TypeName;
				type.CustomAttributes.Clear();
				Target.ManifestModule.Types.Add(type);
				logger.Level--;
				i--;
			}
		}


		private void MergeType()
		{
			for (int j = 0; j < Target.ManifestModule.Types.Count();)
			{
				var type = Target.ManifestModule.Types[j];
				j++;
				foreach(var typeAttr in type.CustomAttributes)
				{
					if (typeAttr.AttributeType.FullName != "PBase.PDependence") continue;
					PDependenceOption o = (PDependenceOption)typeAttr.ConstructorArguments[0].Value;
					bool isClass = (bool)typeAttr.ConstructorArguments[1].Value;
					string name = (UTF8String)typeAttr.ConstructorArguments[2].Value;
					if (isClass && !HasClass(name))
					{
						if (o == PDependenceOption.Error)
							throw new PatchException("[" + type.FullName + "]Error: Dependence named \"" + name + "\" not found");
						else if (o == PDependenceOption.Warning)
							logger.Log("[" + type.FullName + "]Warning: Dependence named \"" + name + "\" not found");
						else if (o == PDependenceOption.Skip)
						{
							logger.Log("[" + type.FullName + "]Skipped: Dependence named \"" + name + "\" not found");
							return;
						}
					}
					else if (!isClass && !HasMethod(name))
					{
						if (o == PDependenceOption.Error)
							throw new PatchException("");
						else if (o == PDependenceOption.Warning)
							logger.Log("[" + type.FullName + "]Warning: Dependence named \"" + name + "\" not found");
						else if (o == PDependenceOption.Skip)
						{
							logger.Log("[" + type.FullName + "]Skipped: Dependence named \"" + name + "\" not found");
							return;
						}
					}
				}
				CustomAttribute typeAttr1 = FirstCustomAttributes(type, "PBase.PPatch");
				if (typeAttr1 == null) continue;
				PPatchOption option = (PPatchOption)typeAttr1.ConstructorArguments[0].Value;
				if (option != PPatchOption.Merge) continue;
				type.CustomAttributes.Remove(typeAttr1);
				logger.Log("[" + option + "] Type founded:\t\"" + type.FullName + "\"");
				string Namespace = (UTF8String)typeAttr1.ConstructorArguments[1].Value;
				string TypeName = (UTF8String)typeAttr1.ConstructorArguments[2].Value;
				string TypeDescription = (UTF8String)typeAttr1.ConstructorArguments[3].Value;
				bool moveStaticVarible = (bool)typeAttr1.ConstructorArguments[4].Value;
				bool moveDynamicVarible = (bool)typeAttr1.ConstructorArguments[5].Value;
				bool moveUnnamedType = (bool)typeAttr1.ConstructorArguments[6].Value;


				logger.Level++;
				if (moveStaticVarible) MoveStaticVarible(type, Namespace + "." + TypeName);
				if (moveDynamicVarible) MoveDynamicVarible(type, Namespace + "." + TypeName);
				if (moveUnnamedType) MoveAnonymousType(type, Namespace + "." + TypeName);
				MergeMethod(type, Namespace + "." + TypeName);
				logger.Level--;

				Target.ManifestModule.Types.Remove(type);
				j--;
			}
		}

		private void MoveStaticVarible(TypeDef type, string locate)
		{
			logger.Log("Moving static fields from \"" + type.FullName + "\" to \"" + locate + "\"");
			var targetType = Target.ManifestModule.Types.First(t => t.FullName == locate);
			for (int i = 0; i < type.Fields.Count();)
			{
				if (type.Fields[i].IsStatic) type.Fields[i].DeclaringType = targetType;
				else i++;
			}
		}

		private void MoveDynamicVarible(TypeDef type, string locate)
		{
			logger.Log("Moving dynamic fields from \"" + type.FullName + "\" to \"" + locate + "\"");
			var targetType = Target.ManifestModule.Types.First(t => t.FullName == locate);
			for (int i = 0; i < type.Fields.Count();)
			{
				if (!type.Fields[i].IsStatic) type.Fields[i].DeclaringType = targetType;
				else i++;
			}
		}

		private void MoveAnonymousType(TypeDef type, string locate)
		{
			logger.Log("Moving anonymous types from \"" + type.FullName + "\" to \"" + locate + "\"");
			var targetType = Target.ManifestModule.Types.First(t => t.FullName == locate);
			for (int i = 0; i < type.NestedTypes.Count();)
			{
				var t = type.NestedTypes[i];
				if (t.Name.StartsWith("<>"))
				{
					t.Name += "_H_" + GetStringHash(type.Name);
					t.DeclaringType = null;
					t.DeclaringType = targetType;
				}
				else i++;
			}
		}

		public CustomAttribute FirstCustomAttributes(ITypeOrMethodDef def, string c)
		{
			foreach (var s in def.CustomAttributes)
				if (s.TypeFullName == c)
					return s;
			return null;
		}


		private void MergeMethod(TypeDef type, string MethodLocate)
		{
			logger.Log("Merging methods......");
			for (int i = 0; i < type.Methods.Count();)
			{
				var method = type.Methods[i];
				var attr = FirstCustomAttributes(method, "PBase.PMethod");
				i++;
				if (attr == null) continue;
				PMethodOption option = (PMethodOption)attr.ConstructorArguments[0].Value;
				string MethodName = (UTF8String)attr.ConstructorArguments[1].Value;
				string Description = (UTF8String)attr.ConstructorArguments[2].Value;
				logger.Level++;
				if (option == PMethodOption.Move)
				{
					MoveMethod(MethodLocate, method);
					i--;
				}
				logger.Level--;
			}
			for (int i = 0; i < type.Methods.Count();)
			{
				var method = type.Methods[i];
				var attr = FirstCustomAttributes(method, "PBase.PMethod");
				i++;
				if (attr == null) continue;
				PMethodOption option = (PMethodOption)attr.ConstructorArguments[0].Value;
				string MethodName = (UTF8String)attr.ConstructorArguments[1].Value;
				string Description = (UTF8String)attr.ConstructorArguments[2].Value;
				logger.Level++;
				if (option == PMethodOption.Replace)
				{
					ReplaceMethod(MethodLocate, MethodName, method);
					i--;
				}
				logger.Level--;
			}
			for (int i = 0; i < type.Methods.Count();)
			{
				var method = type.Methods[i];
				var attr = FirstCustomAttributes(method, "PBase.PMethod");
				i++;
				if (attr == null) continue;
				PMethodOption option = (PMethodOption)attr.ConstructorArguments[0].Value;
				string MethodName = (UTF8String)attr.ConstructorArguments[1].Value;
				string Description = (UTF8String)attr.ConstructorArguments[2].Value;
				logger.Level++;
				if (option == PMethodOption.Hook)
				{
					HookMethod(MethodLocate, MethodName, method, GetStringHash(type.Name));
					i--;
				}
				logger.Level--;
			}
		}

		private void MoveMethod(string MethodLocate, MethodDef md)
		{
			var targetType = Target.ManifestModule.Types.First(t => t.FullName == MethodLocate);
			var methods = targetType.Methods.Where(m => m.Name == md.Name);
			bool flag = false;
			if (methods.Count() != 0)
			{
				foreach (var m in methods)
				{
					if (md.IsStatic == m.IsStatic)
						flag = true;
					if (MatchMethod(m, md, md.IsStatic))
						flag = true;
				}
			}
			if (flag)
			{
				logger.Log("There is another exists method liked this");
				throw new PatchException("A method with Patcher.PMethod::PMethodOption.Modify must be with one parameter");
			}
			md.CustomAttributes.Clear();
			md.DeclaringType = targetType;
		}

		private void ReplaceMethod(string MethodLocate, string MethodName, MethodDef md)
		{
			logger.Log("Replacing method \"" + MethodLocate + "::" + MethodName + "\" with \"" + md.DeclaringType.Namespace + "::" + md.Name + "\"");
			var targetType = Target.ManifestModule.Types.First(t => t.FullName == MethodLocate);
			MethodDef targetMethod = targetType.Methods.First(t => (t.Name == MethodName && MatchMethod(t, md, md.IsStatic)));
			md.DeclaringType = targetType;
			if (!md.IsStatic) md.Parameters[0].Type = targetMethod.Parameters[0].Type;
			//md.CustomAttributes.Clear();
			logger.Log("Replacing method references...");
			int nrmr = 0;
			for (int i = 0; i < Target.ManifestModule.Types.Count(); i++)
			{
				var type = Target.ManifestModule.Types[i];
				nrmr += ReplaceMethodRef(type, targetMethod, md);
			}
			logger.Log("Number of replaced method references:\t" + nrmr);

		}

		private void HookMethod(string MethodLocate, string MethodName, MethodDef md, string hash)
		{
			logger.Log("Hooking method \"" + MethodLocate + "::" + MethodName + "\" with \"" + md.DeclaringType.Namespace + "::" + md.Name + "\"");
			if ((md.IsStatic && md.Parameters.Count() != 0) || ((!md.IsStatic) && md.Parameters.Count() != 1))
			{
				logger.Log(">>Error:\tcan't be hooked with method which has parameters");
				throw new PatchException("Can't be hooked with method which has parameters");
			}
			var targetType = Target.ManifestModule.Types.First(t => t.FullName == MethodLocate);
			md.DeclaringType = targetType;
			md.Name = md.Name + "_P" + "_" + hash;
			//md.CustomAttributes.Clear();
			MethodDef targetMethod = targetType.Methods.First(t => t.Name == MethodName && MatchMethod(t, md, md.IsStatic));
			if (!md.IsStatic) md.Parameters[0].Type = targetMethod.Parameters[0].Type;
			md.Attributes = md.Attributes & (~MethodAttributes.SpecialName) & (~MethodAttributes.RTSpecialName);
			IList<Instruction> inst = targetMethod.Body.Instructions;
			if (!md.IsStatic)
			{
				inst[inst.Count() - 1] = Instruction.Create(OpCodes.Ldarg_0);
				inst.Add(Instruction.Create(OpCodes.Call, md));
				inst.Add(Instruction.Create(OpCodes.Ret));
			}
			else
			{
				inst[inst.Count() - 1] = Instruction.Create(OpCodes.Call, md);
				inst.Add(Instruction.Create(OpCodes.Ret));
			}
		}

		private bool MatchMethod(MethodDef a, MethodDef b, bool isStatic)
		{
			if (a.Parameters.Count() != b.Parameters.Count()) return false;
			for (int i = isStatic ? 0 : 1; i < a.Parameters.Count(); i++)
				if (a.Parameters[i].Type.ToString() != b.Parameters[i].Type.ToString())
					return false;
			return true;
		}
		private int ReplaceMethodRef(TypeDef type, MethodDef target, MethodDef src)
		{
			int number = 0;
			if (type.HasNestedTypes)
				foreach (var t in type.NestedTypes)
					ReplaceMethodRef(t, target, src);
			foreach (var method in type.Methods)
			{
				var attr = FirstCustomAttributes(method, "PBase.PMethod");
				if (attr != null)
					if (!(bool)attr.ConstructorArguments[3].Value)
						continue;
				if (method.HasBody)
				{
					for (int i = 0; i < method.Body.Instructions.Count(); i++)
					{
						var IL = method.Body.Instructions[i];
						if (IL.OpCode == OpCodes.Call)
						{
							IMethod m = (IMethod)IL.Operand;
							if (m.FullName == target.FullName)
							{
								IL.Operand = src;
								number++;
							}
						}
						else if (IL.OpCode == OpCodes.Ldftn)
						{
							IMethod m = (IMethod)IL.Operand;
							if (m.FullName == target.FullName)
							{
								if (m is MethodDef)
								{
									IL.Operand = src;
									number++;
								}
							}

						}
					}
				}
			}
			return number;
		}
		public static void ExportPublic(string target)
		{
			PatchTool pt = new PatchTool();
			pt.Target = AssemblyDef.Load(target);
			pt.PublicAllMember();
			pt.Save("Public.exe");
		}
		public static void ExportTerrariaLibrary(string FileName)
		{
			IEnumerable<Resource> arg_2F_0 = AssemblyDef.Load(FileName).ManifestModule.Resources;
			using (IEnumerator<Resource> enumerator = arg_2F_0.Where(r => r.Name.Contains(".dll")).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					EmbeddedResource embeddedResource = (EmbeddedResource)enumerator.Current;
					string a = embeddedResource.Name;
					if (!(a == "Terraria.Libraries.DotNetZip.Ionic.Zip.CF.dll"))
						if (!(a == "Terraria.Libraries.JSON.NET.Newtonsoft.Json.dll"))
							if (!(a == "Terraria.Libraries.ReLogic.ReLogic.dll"))
								if (a == "Terraria.Libraries.Steamworks.NET.Windows.Steamworks.NET.dll")
									File.WriteAllBytes("Steamworks.NET.dll", embeddedResource.GetResourceData());
								else
									File.WriteAllBytes("ReLogic.dll", embeddedResource.GetResourceData());
							else
								File.WriteAllBytes("Newtonsoft.Json.dll", embeddedResource.GetResourceData());
						else
							File.WriteAllBytes("Ionic.Zip.CF.dll", embeddedResource.GetResourceData());
				}
			}
		}
		public void PublicAllMember()
		{
			Action<TypeDef> p = null;
			p = (type) =>
			{
				if (type.Name.Contains("PrivateImplementationDetails"))
					return;
				if (type.DeclaringType != null)
					type.Attributes = (type.Attributes & (~TypeAttributes.NestedPrivate)) | TypeAttributes.NestedPublic;
				else
					type.Attributes |= TypeAttributes.Public;
				if (type.HasNestedTypes)
					foreach (var t in type.NestedTypes)
						p(t);
				foreach (var t in type.GetTypes())
					p(t);
				foreach (var method in type.Methods)
				{
					if (method.IsCompilerControlled) continue;
					method.Attributes &= ~MethodAttributes.Private;
					method.Attributes |= MethodAttributes.Public;
				}
				foreach (var field in type.Fields)
				{
					if (field.IsCompilerControlled) continue;
					if (type.FindEvents(field.Name).Count() != 0) continue;
					field.Attributes &= ~FieldAttributes.Private;
					field.Attributes |= FieldAttributes.Public;
				}
			};

			foreach (var type in Target.ManifestModule.Types)
				p(type);
		}
		public string GetStringHash(string str)
		{
			return string.Format("{0:x}", str.GetHashCode());
		}
	}
}
