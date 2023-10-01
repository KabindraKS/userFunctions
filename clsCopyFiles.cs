
/* Comment lines generated by ChatGPT version 3.5
 * This method is designed to copy files from a source folder to a destination folder based on certain filter criteria and settings. 
This method takes the following parameters:
srcFolder: The source folder from which files are to be copied.
destFolder: The destination folder where files will be copied to.
uExt: A file extension filter, specifying which files to include in the copy operation.
ufilters: An array of strings representing file name filters. Only files with names containing these filters will be copied. if nothing, provide empty list.
subDir: An optional subdirectory filter. If specified, only files located in this subdirectory will be copied. if nothing, provide empty list.
eFloders: An array of strings representing excluded folders. Files within these folders will not be copied. if nothing, provide empty list.
cOutFile: Save the item name included in the uFilters and not found in the source folder.

Initialization and Setup:
bool copyAll = false: A flag to determine if all files should be copied.

Initial Checks:
if (!IsFolderExists(cOutFile)) return;: It checks whether the destination folder exists. If it doesn't, the method exits early.
Filter Lists: List of key words to apply filter criteria obtained from the another function provided below.

File Listing:
string[] files = Directory.GetFiles(srcFolder, uExt, SearchOption.AllDirectories);: 
It retrieves an array of file paths from the srcFolder and its subdirectories based on the specified file extension (uExt).

Iterating Through Files:
The code iterates through each file path in the files array.
It checks various conditions and filters to decide whether to copy the file to the destination folder.
The filtering process includes checks for excluded folders, file name filters, and the optional subdirectory filter.
If the file passes all filtering criteria, it is copied to the destination folder.

Writing to Output File:
The code creates a StreamWriter named csvOut to write to an output file specified by cOutFile.
It then removes found items from the itmList dictionary.
Finally, it writes the remaining items in the itmList dictionary to the output file, presumably as a CSV list.

Cleanup:
The StreamWriter is closed.
The status label is cleared.

Overall:
This code essentially copies files from a source folder to a destination folder while applying various filters and conditions. 
The specific filters and criteria for copying files are determined by the parameters and filter lists provided to the method. 
Additionally, it keeps track of which items were copied and which were not in an output CSV file.
 */

private void Copy_Files(string srcFolder, string destFolder, string uExt,
            string[] ufilters, string subDir, string[] eFloders, string cOutFile)
{
    bool copyAll = false;
    if (!IsFolderExists(cOutFile)) return;
    if (ufilters.Length < 1 && eFloders.Length < 1) copyAll = true;

    Dictionary<string, string> itmList = GetFilterList();
    List<string> foundItms = new List<string>();
    //lblStatus.Text = "Listing all files that satisfy the filter criteria, please wait.......";
    //lblStatus.Refresh();
    string[] files = Directory.GetFiles(srcFolder, uExt, SearchOption.AllDirectories);
    foreach (string uFile in files)
    {
        string ufileName = Path.GetFileName(uFile);
        lblStatus.Text = "Checking file....." + ufileName;
        lblStatus.Refresh();
        if (!copyAll)
        {
            bool blnCopy = false;
            bool blnExlcude = false;
            foreach (string uf in eFloders)
            {
                if (uf.Trim() == string.Empty) continue;
                if (uFile.ToUpper().Contains(uf.ToUpper()))
                {
                    blnExlcude = true;
                    continue;
                }
            }
            if (blnExlcude) continue;//iterate for another file.
            foreach (string uf in ufilters)
            {
                if (ufileName.ToUpper().Contains(uf.ToUpper()))
                {
                    if (itmList.Count < 1)
                    {
                        blnCopy = true;
                        break;
                    }
                    string basinNa = ufileName.Substring(0, 5).ToUpper();
                    if (itmList.ContainsKey(basinNa))
                    {
                        blnCopy = true;
                        foundItms.Add(basinNa);//marking as already copied.
                        break;
                    }
                }
            }
            if (subDir != string.Empty && !uFile.Contains(subDir)) blnCopy = false;
            if (!blnCopy) continue;
        }
        string destPath = destFolder + "\\" + ufileName;
        if (File.Exists(destPath)) continue;//do not overwrite.
        File.Copy(uFile, destPath, true);//overwrite the files.
    }
    StreamWriter csvOut = new StreamWriter(cOutFile);
    //remove found items from the collection.
    foreach (string itm in foundItms)
        itmList.Remove(itm);
    //Save the not found items.
    foreach (string basinName in itmList.Values)
        csvOut.WriteLine(basinName);
    csvOut.Close();
    //lblStatus.Text = "";
    //lblStatus.Refresh();
}

/*
This method is used to read a CSV file, where each line typically represents an item, 
and populate a dictionary with these items. The keys in the dictionary are the items converted to uppercase, 
and the values are also the items themselves. This can be useful for maintaining a list of filter criteria or items to be used in other parts of the program.
*/
private Dictionary<string, string> GetFilterList(string fileName)
{
	Dictionary<string, string> filterList = new Dictionary<string, string>();
	 
	if (!File.Exists(fileName)) return filterList;

	StreamReader csvFile = new StreamReader(fileName);
	csvFile.ReadLine();
	while (!csvFile.EndOfStream)
	{   string itmName = csvFile.ReadLine();
		string[] itms = itmName.Split(',');
		if (itms.Length < 1) continue;
		string itm = itms[0].ToUpper();
		if (!filterList.ContainsKey(itm))
			filterList.Add(itm, itm);}
	csvFile.Close();
	return filterList;
}