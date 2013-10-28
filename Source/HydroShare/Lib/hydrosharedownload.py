import urllib2
import os
import json
import zipfile
import shutil

class HydroshareDownloader():
    #base_url points to the Export.php script that packages data files for download.
    base_url="http://dev.hydroshare.org/export.php?file=/home/hydroadmin/hydroshare/sites/default/files/{0}.zip"
    	
    #The url from which a list of available resources can be found
    list_url="http://dev.hydroshare.org/?q=my_services/node.json"

    #Local filepath where the downloaded files will be saved
    file_path=""

    def downloadFile(self, resource):
        '''Downloads a file with given name from HydroShare and saves it to the specified location.'''

        #Remove spaces from Resource name
        resource = resource.replace(" ", "")
        	
        #Combine the Save folder with the resource name
        save_loc = os.path.join(self.file_path, resource + ".zip")
        	
        #Download and save the file
        file = urllib2.urlopen(self.base_url.format(resource))
        with open(save_loc, "wb") as code:
            code.write(file.read())
        
        #Unzip the data in save_loc with the given name resource.
        self.unzipData(save_loc, resource)

    def retrieveList(self, filter=""):
        '''Download list of files as JSON string and filters it to return list of available files.'''
        #Optional filter argument could later be used to know which list to return.

        #Load the data from the url into data
        data = urllib2.urlopen(self.list_url)

        #Turn the data from a raw string into JSON
        all_files = json.load(data)

        #Filtered_files is used as a hack to filter the list to see just the files we want
        filtered_files = []
        
        #Look through all nodes in the JSON data
        for file in all_files:
            
            #This if-elif-else block doesn't seem to be working very well, but I'm leaving it in anyways.
            #Basically we just return the correct list based on the "filter" parameter passed in, but for whatever reason only
            #"Spatial Data" works, which is actually good for now because that's the only ones that we can download, extract, and open in HydroDesktop for now.
            if (filter == u"hydroshare_geoanalytics"):
                if (file["type"] == u"hydroshare_geoanalytics"):
                    filtered_files.append(file["title"])
            elif (filter == u"hydroshare_time_series"):
                if (file["type"] == "hydroshare_time_series"):
                    filter_files.append(file["title"])
            else:
                #If the node is one of these two types then we will show it in our list
                #if (file["type"] == "hydroshare_time_series" 
                #or file["type"] == "hydroshare_geoanalytics"):
                    #Access the node's title through the JSON key "title"
                    filtered_files.append(file["title"])
        
        return filtered_files

    def unzipData(self, saved_loc, resource):
        '''Unzip the downloaded files and move them to a permanent home'''
        #Create a ZipFile object from the saved file.
        zipped = zipfile.ZipFile(saved_loc)

        #Create two folders, one temporary "result" folder and one folder with the same name as the resource.
        #The final_save_path folder named after the resource will be used to store the data files that are extracted.
        temp_path = os.path.join(self.file_path, "hydro_share_download_result_temp_folder")
        final_save_path = os.path.join(self.file_path, resource)

        #If the temp folder exists does just delete it.
        if os.path.exists(temp_path):
            shutil.rmtree(temp_path)
        os.makedirs(temp_path)

        #Make sure the final save path folder doesn't exist already before we create it
        #TODO: If the folder does already exist (for example, if the user already downloaded this data),
        #then we are simply going to overwrite any files that we download with the same name. 
        #This might not be ideal.
        if not os.path.exists(final_save_path):
            os.makedirs(final_save_path)

        #Extract all files to the temporary folder and then close the file.
        zipped.extractall(temp_path)
        zipped.close()

        #Check if there is another .zip file containing the data, if so, unzip it to the final folder.
        for f in os.listdir(os.path.join(temp_path, "data")):
            #Extract any and all files with .zip that we find in the results/data folder.
            if f.endswith(".zip"):
                data_zipped = zipfile.ZipFile(os.path.join(temp_path, "data", f))
                #Extract these files to the final save folder, then close the zip file.
                data_zipped.extractall(final_save_path)
                data_zipped.close()
                #Print the filepath of the final save folder back to HydroDesktop so Hydrodesktop knows where to look for files
                print final_save_path
        #Copy metadata file to the final_save_path
        shutil.copyfile(os.path.join(temp_path, "data", "sciencemetadata.xml"), os.path.join(final_save_path, "sciencemetadata.xml"))
        #Remove the temporary folder when finished.
        shutil.rmtree(temp_path)

    def test(self):
        '''Test this script.'''
        self.test_retrieveList()
        self.test_downloadFile()

    def test_downloadFile(self):
        '''Test method that saves a test file to same folder as script'''
        self.downloadFile("EdgarRanchComparisonTest1")

    def test_retrieveList(self):
        '''Test method that retrieves and prints the list of files from HydroShare'''
        list = self.retrieveList()
        for file in list:
            print(file)

#Run test when script is run from the command line
if __name__ == '__main__':
    tester = HydroshareDownloader()
    tester.test()