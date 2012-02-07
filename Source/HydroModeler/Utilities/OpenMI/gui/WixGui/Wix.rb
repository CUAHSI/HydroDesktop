def make_installer
	candle='C:\Program Files\Windows Installer XML\bin\candle'
	light='C:\Program Files\Windows Installer XML\bin\light'
	wixUI='"C:\Program Files\Windows Installer XML\bin\wixUI.wixlib"'
	wixUIenUS='"C:\Program Files\Windows Installer XML\bin\WixUI_en-us.wxl"'
	
#  product = "Oatc.OpenMI.Gui.ConfigurationEditor 1.4 Alpha 5"
#	xmlFiles = ["Gui_1.4_Alpha5"]
#  product = "Oatc.OpenMI.Gui.ConfigurationEditor 1.4 Beta 1"
#	xmlFiles = ["Gui_1.4_Beta1"]
#  product = "Oatc.OpenMI.Gui.ConfigurationEditor 1.4 Beta 2"
#	xmlFiles = ["Gui_1.4_Beta2"]
  product = "Oatc.OpenMI.Gui.ConfigurationEditor 1.4"
	xmlFiles = ["Gui_1.4_Release"]

	wixs = []
	wixobjs = []

	xmlFiles.each { |f| wixs.push f + ".wxs" }
	xmlFiles.each { |f| wixobjs.push f + ".wixobj" }

	return if !system_command("Making Candle", "#{candle} #{wixs.join(" ")}")  
	return if !system_command("Making Light",  "#{light}  #{wixobjs.join(" ")} #{wixUI} -loc #{wixUIenUS} -out \"#{product}.msi\"")  

	STDOUT.puts "Installer Make completed OK"
	STDOUT.flush
end

def system_command(comment, cmd)
	STDOUT.puts "\n#{comment} ...\n"    
	STDOUT.flush
	STDOUT.puts `#{cmd}`
	return true if $? == 0
	STDERR.puts "\n===FAILED=== #{comment}\n"
	return false
end

make_installer()



