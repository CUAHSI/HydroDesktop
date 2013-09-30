import urllib2
import os
import json
import zipfile

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
        
        self.unzipData(save_loc, resource)

    def retrieveList(self):
        '''Download list of files as JSON string and filters it to return list of available files.'''
        data = urllib2.urlopen(self.list_url)

        all_files = json.load(data)
        filtered_files = []
        	
        for file in all_files:
            if (file["type"] == "hydroshare_time_series" 
            or file["type"] == "hydroshare_geoanalytics"):
                filtered_files.append(file["title"])
        
        return filtered_files

    def unzipData(self, saved_loc, resource):
        zipped = zipfile.ZipFile(saved_loc)
        temp_path = os.path.join(self.file_path, "result")
        save_path = os.path.join(self.file_path, resource)
        try:
            os.makedirs(temp_path)
            os.makedirs(save_path)
        except OSError as exception:
            if exception.errno != errno.EEXIST:
                raise

        zipped.extractall(temp_path)
        data_zipped = zipfile.ZipFile(os.path.join(temp_path, data, )

        
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