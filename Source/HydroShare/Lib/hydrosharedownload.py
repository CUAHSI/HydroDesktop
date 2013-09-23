import urllib2
import os
import json

#base_url points to the Export.php script that packages data files for download.
base_url="http://dev.hydroshare.org/export.php?file=/home/hydroadmin/hydroshare/sites/default/files/{0}.zip"
	
#The url from which a list of available resources can be found
list_url="http://dev.hydroshare.org/?q=my_services/node.json"

#Local filepath where the downloaded files will be saved
file_path=""

def downloadFile(resource, save_loc=file_path):
	'''Downloads a file with given name from HydroShare and saves it to the specified location.'''

	#Remove spaces from Resource name
	resource = resource.replace(" ", "")
	
	#Combine the Save folder with the resource name
	save_loc = os.path.join(save_loc, resource + ".zip")
	
	#Download and save the file
	file = urllib2.urlopen(base_url.format(resource))
	with open(save_loc, "wb") as code:
		code.write(file.read())

def retrieveList():
	'''Download list of files as JSON string and filters it to return list of available files.'''
	data = urllib2.urlopen(list_url)

	all_files = json.load(data)
	filtered_files = []
	
	for file in all_files:
		if (file["type"] == "hydroshare_time_series" 
		or file["type"] == "hydroshare_geoanalytics"):
			filtered_files.append(file["title"])
			
	return filtered_files

def test():
	'''Test this script.'''
	test_retrieveList()
	test_downloadFile()

def test_downloadFile():
	'''Test method that saves a test file to same folder as script'''
	downloadFile("TimeSeriesTest", "")

def test_retrieveList():
	'''Test method that retrieves and prints the list of files from HydroShare'''
	list = retrieveList()
	for file in list:
		print(file)

#Run test when script is run from the command line
if __name__ == '__main__':
	test()