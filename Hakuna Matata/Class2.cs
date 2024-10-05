using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.CSharp;

namespace CustomWindowsForm
{
	// Token: 0x02000015 RID: 21
	public class Compiler
	{
		// Token: 0x060000D6 RID: 214 RVA: 0x00008C30 File Offset: 0x00006E30
		public Compiler(string sourceCode, string savePath, string iconLocation, string messagebox)
		{
			string[] array = new string[] { "System.dll", "System.Linq.dll", "System.Windows.Forms.dll", "Microsoft.CSharp.dll", "System.Text.RegularExpressions.dll", "System.Runtime.InteropServices.dll", "System.Security.dll", "System.Drawing.dll", "System.Security.Principal.dll" };
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };
			string text = "/target:winexe /platform:anycpu /optimize+ ";
			bool flag = iconLocation != "";
			if (flag)
			{
				text = text + "/win32icon:" + iconLocation;
			}
			using (CSharpCodeProvider csharpCodeProvider = new CSharpCodeProvider(dictionary))
			{
				CompilerParameters compilerParameters = new CompilerParameters(array)
				{
					GenerateExecutable = true,
					GenerateInMemory = false,
					OutputAssembly = savePath,
					CompilerOptions = text,
					TreatWarningsAsErrors = false,
					IncludeDebugInformation = false
				};
				CompilerResults compilerResults = csharpCodeProvider.CompileAssemblyFromSource(compilerParameters, new string[] { sourceCode });
				bool flag2 = compilerResults.Errors.Count > 0;
				if (flag2)
				{
					foreach (object obj in compilerResults.Errors)
					{
						CompilerError compilerError = (CompilerError)obj;
						MessageBox.Show(string.Format("{0}\nLine: {1} - Column: {2}\nFile: {3}", new object[] { compilerError.ErrorText, compilerError.Line, compilerError.Column, compilerError.FileName }));
					}
				}
				else
				{
					MessageBox.Show(messagebox);
				}
			}
		}
	}
}
