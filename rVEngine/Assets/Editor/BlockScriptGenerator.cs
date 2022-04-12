using BlockUtils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BlockUtils
{
    public class BlockScriptGenerator
    {
        public static void Register()
        {
            if (Application.isEditor)
            {
                bool value = EditorUtility.DisplayDialog(
                       "Register Blocks",
                       "Do you really want to register all the blocks?",
                       "Yes",
                       "No"
                       );


                if (value)
                {
                    RegisterCode();
                }
            }
        }

        private static void ClearFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private static void RegisterCode()
        {
            string path = $"{Application.dataPath}/Scripts/Engine/Block/Block/BlockRegister.cs";
            List<string> codeLines = new List<string>();

            codeLines.Add("public class BlockRegister");
            codeLines.Add("{");

            List<Block> blocks = BlockCollector.GetExistingBlocks();

            int regularIndex = 0;
            int specialIndex = 1;

            for (int i = 0; i < blocks.Count; i++)
            {
                Block block = blocks[i];
                if (!block.IsHasCustomMesh)
                {
                    if (block.IsCollided)
                    {
                        codeLines.Add($"public static int {block.RegisterTitle} = {regularIndex};");
                        regularIndex++;
                    }
                }
                else 
                {
                    if (block.IsCollided)
                    {
                        codeLines.Add($"public static int {block.RegisterTitle} = {-specialIndex};");
                        specialIndex++;
                    }
                }
            }

            for (int i = 0; i < blocks.Count; i++)
            {
                Block block = blocks[i];
                if (!block.IsHasCustomMesh)
                {
                    if (!block.IsCollided)
                    {
                        codeLines.Add($"public static int {block.RegisterTitle} = {regularIndex};");
                        regularIndex++;
                    }
                }
                else
                {
                    if (!block.IsCollided)
                    {
                        codeLines.Add($"public static int {block.RegisterTitle} = {-specialIndex};");
                        specialIndex++;
                    }
                }
            }

            codeLines.Add("}");

            using (StreamWriter outputFile = new StreamWriter(path))
            {
                foreach (string line in codeLines.ToArray())
                    outputFile.WriteLine(line);
            }

        }
    }
}