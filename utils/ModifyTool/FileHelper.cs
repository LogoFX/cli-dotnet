using System;
using System.IO;

namespace ModifyTool
{
    internal static class FileHelper
    {
        internal static void CreateFile(string folderName, string filePath, Action<string> replaceSolutionNameMethod)
        {
            if (!File.Exists(filePath))
            {
                var helper = new ResourceHelper(folderName, Consts.ModuleFileName);

                using var stream = helper.GetResourceStream();
                using (var fileStream = File.Create(filePath))
                {
                    stream.CopyTo(fileStream);
                }

                replaceSolutionNameMethod(filePath);
            }
        }
    }
}
