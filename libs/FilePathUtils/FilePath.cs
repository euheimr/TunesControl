using System;
using System.Collections.Generic;
using System.IO;

public class FilePathLib
{

    public static string DirSlash(string path)
    {

        // determine if forward slash or back slash
        string slash;
        if (path.Contains(@"/"))
        { slash = @"/"; }
        else
        { slash = @"\"; }

        // add a slash to the end of a string if it doesn't exist
        if (path.EndsWith(slash) == false)
        {
            path += slash;
        }

        // replaces multiple slashes with a single slash at the end of the string
        while (path.EndsWith(slash + slash))
        {
            path = path.Remove(path.Length - 1, 1);
        }

        return path;

    }


    public static void DeleteFile(string filePath)
    {

        File.SetAttributes(filePath, System.IO.FileAttributes.Normal);
        File.Delete(filePath);

    }

    public static string ReadTextFile(string filePath)
    {
        string fileData = string.Empty;
        TextReader tr = null;
        tr = System.IO.File.OpenText(filePath);
        fileData = tr.ReadToEnd();
        tr.Close();
        return fileData;
    }

    public static void WriteTextFile(string filePath, string fileData, bool appendData)
    {
        TextWriter tw = null;
        if (appendData == true)
        {
            tw = System.IO.File.AppendText(filePath);
        }
        else
        {
            tw = System.IO.File.CreateText(filePath);
        }
        tw.Write(fileData);
        tw.Flush();
        tw.Close();
    }

    public static void WriteTextFileLine(string filePath, string lineData)
    {
        TextWriter tw = null;
        tw = System.IO.File.AppendText(filePath);
        tw.WriteLine(lineData);
        tw.Flush();
        tw.Close();
    }

    public static void PrependLineToTextFile(string filePath, string lineData)
    {
        string fileData = ReadTextFile(filePath);
        filePath = lineData + System.Environment.NewLine + fileData;
        WriteTextFile(filePath, fileData, false);
    }

    public static void DeleteOldFiles(string path, DateTime compareDate, string searchPattern, ref List<string> filesDeleted, bool checkAccessTimeToo = true)
    {

        foreach (string filePath in System.IO.Directory.GetFiles(path, searchPattern))
        {

            FileInfo fileInfo = new System.IO.FileInfo(filePath);

            // use whichever one is newest
            DateTime lastDate = fileInfo.LastWriteTime;
            if (checkAccessTimeToo && fileInfo.LastAccessTime > lastDate)
            {
                lastDate = fileInfo.LastAccessTime;
            }

            // if older than compare date
            if (lastDate < compareDate)
            {
                if (filesDeleted != null)
                { filesDeleted.Add(filePath); }

                DeleteFile(filePath);
            }
        }

    }

    public static FileInfo FindNewestFileByLastWriteDate(string path, string searchPattern)
    {
        FileInfo newest = null;
        foreach (string filePath in System.IO.Directory.GetFiles(path, searchPattern))
        {
            FileInfo fi = new FileInfo(filePath);
            if (newest == null || fi.LastWriteTime > newest.LastWriteTime)
            {
                newest = fi;
            }

        }

        return newest;
    }

    public class SyncDirectoryResults
    {
        public int FilesAdded { get; set; }
        public int FilesUpdated { get; set; }
        public int FilesDeleted { get; set; }
        public int DirectoriesCreated { get; set; }
        public int DirectoriesDeleted { get; set; }
    }

    public static SyncDirectoryResults SyncDirectory(string sourcePath, string destPath, string searchPattern = "*.*", List<string> excludePaths = null)
    {
        SyncDirectoryResults results = new SyncDirectoryResults();
        SyncDirectory(ref results, sourcePath, destPath, searchPattern, excludePaths);
        return results;
    }

    public static void SyncDirectory(ref SyncDirectoryResults results, string sourcePath, string destPath, string searchPattern = "*.*", List<string> excludePaths = null)
    {

        sourcePath = DirSlash(sourcePath);
        destPath = DirSlash(destPath);

        // something is wrong if the source path isnt there
        if (!Directory.Exists(sourcePath))
        {
            throw new Exception("Path not found \"" + sourcePath + "\"");
        }

        // create the destination if it doesnt exist
        if (!Directory.Exists(destPath))
        {
            Directory.CreateDirectory(destPath);
            results.DirectoriesCreated++;
        }

        // copy newer files from source
        foreach (string sourceFilePath in Directory.GetFiles(sourcePath, searchPattern))
        {

            // skip if excluded
            if (IsPathExcluded(sourceFilePath, excludePaths))
            {
                continue;
            }

            FileInfo sourceFileInfo = new FileInfo(sourceFilePath);

            string destFilePath = destPath + sourceFileInfo.FullName.Remove(0, sourcePath.Length);

            if (File.Exists(destFilePath))
            {
                FileInfo destFileInfo = new FileInfo(destFilePath);
                if (sourceFileInfo.LastWriteTime != destFileInfo.LastWriteTime || sourceFileInfo.Length != destFileInfo.Length)
                {
                    DeleteFile(destFilePath);
                    File.Copy(sourceFilePath, destFilePath);
                    results.FilesUpdated++;
                }

            }
            else
            {
                File.Copy(sourceFilePath, destFilePath);
                results.FilesAdded++;
            }

        }

        // remove files from dest that arent in source
        foreach (string destFilePath in Directory.GetFiles(destPath, searchPattern))
        {

            // skip if excluded
            if (IsPathExcluded(destFilePath, excludePaths))
            {
                continue;
            }

            FileInfo destFileInfo = new FileInfo(destFilePath);

            string sourceFilePath = sourcePath + destFileInfo.FullName.Remove(0, destPath.Length);

            if (!File.Exists(sourceFilePath))
            {
                DeleteFile(destFilePath);
                results.FilesDeleted++;
            }
        }

        // remove directories from dest that arent in source
        foreach (string destDirPath in Directory.GetDirectories(destPath))
        {

            // skip if excluded
            if (IsPathExcluded(destDirPath, excludePaths))
            {
                continue;
            }

            string destDir = DirSlash(destDirPath);

            string sourceDirPath = sourcePath + destDir.Remove(0, destPath.Length);

            if (!Directory.Exists(sourceDirPath))
            {
                Directory.Delete(destDir, true);
                results.DirectoriesDeleted++;
            }
        }

        // recurse child directories
        foreach (string sourceDirPath in Directory.GetDirectories(sourcePath))
        {
            // skip if excluded
            if (IsPathExcluded(sourceDirPath, excludePaths))
            {
                continue;
            }

            string sourceDir = DirSlash(sourceDirPath);
            string destDir = destPath + sourceDir.Remove(0, sourcePath.Length);
            SyncDirectory(ref results, sourceDir, destDir, searchPattern, excludePaths);
        }

    }



    public static bool IsPathExcluded(string path, List<string> excludePaths)
    {
        if (excludePaths == null)
        {
            return false;
        }

        foreach (string excludePath in excludePaths)
        {

            // dirslash is for directories, but since this is just a comparison, this works for files too
            if (DirSlash(path.ToLower().Trim()).EndsWith(DirSlash(excludePath.ToLower().Trim())))
            {
                return true;
            }

        }

        return false;

    }

}


