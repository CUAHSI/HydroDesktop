﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DroughtAnalysis
{
    /// <summary>
    /// Modifies the R-script by setting the output directory, input file path and station name 
    /// parameters.
    /// </summary>
    public static class RScriptModifier
    {
        /// <summary>
        /// Modifies the R-script and saves it to the output path
        /// </summary>
        /// <param name="inputScriptPath">The input script path</param>
        /// <param name="outputScriptPath">The output script path</param>
        /// <param name="parameters">The parameters to be modified</param>
        public static void ModifyRScript(string inputScriptPath, string outputScriptPath, RScriptParameterInfo parameters)
        {
            if (!File.Exists(inputScriptPath))
                throw new FileNotFoundException("The R script '" + inputScriptPath + "' doesn't exist!");
            
            //check the parameters passed in
            //param 1 working directory
            if (!Directory.Exists(parameters.OutputDirectory))
                throw new DirectoryNotFoundException(string.Format("The Directory '{0}' doesn't exist!"));
            //param 2 the input file
            if (!File.Exists(parameters.InputFilePath))
                throw new FileNotFoundException(string.Format("The input file '{0}' doesn't exist!"));
            //param 3 the station name
            if (String.IsNullOrWhiteSpace(parameters.StationName))
                throw new ArgumentException("The station name is not specified!");

            string line = string.Empty;
            bool inputFileParamReplaced = false; //this stays false until all required
            bool stationNameParamReplaced = false; //this stays false until the station name param is replaced.

                                                //parameters are replaced.
            using (StreamReader reader = new StreamReader(inputScriptPath))
            {
                using (StreamWriter writer = new StreamWriter(outputScriptPath, false))
                {
                    //add first line which sets the proper working directory
                    string workDir = parameters.OutputDirectory.Replace(@"\", "/");
                    writer.WriteLine("setwd('" + workDir + "')");
                    
                    
                    while ((line = reader.ReadLine()) != null)
                    {
                        //replace line with nazev stanice
                        if (!stationNameParamReplaced)
                        {
                            if (line.Contains("Nazevstanice ="))
                            {
                                line = string.Format("Nazevstanice = '{0}'", parameters.StationName);
                                stationNameParamReplaced = false;
                            }
                        }
                        //replace line with soubor (this is the input file path
                        if (!inputFileParamReplaced)
                        {
                            if (line.Contains("soubor ="))
                            {
                                string inputFile = parameters.InputFilePath.Replace(@"\","/");
                                line = string.Format("soubor = '{0}'", inputFile);
                            }
                        }

                        writer.WriteLine(line);
                    }
                }
            }
        }
        /// <summary>
        /// Removes any diacritic characters and replaces them by ascii characters (important for the file name)
        /// </summary>
        /// <param name="Text">the text in unicode characters</param>
        /// <returns>the converted text in ascii characters</returns>
        public static string RemoveDiacritism(string Text)
        {
            string stringFormD = Text.Normalize(System.Text.NormalizationForm.FormD);
            System.Text.StringBuilder retVal = new System.Text.StringBuilder();
            for (int index = 0; index < stringFormD.Length; index++)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stringFormD[index]) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    retVal.Append(stringFormD[index]);
            }
            return retVal.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }
    }
    /// <summary>
    /// Parameters of the R script
    /// </summary>
    public class RScriptParameterInfo
    {
        /// <summary>
        /// Gets or sets the output directory
        /// </summary>
        public string OutputDirectory { get; set; }
        /// <summary>
        /// Gets or sets the input file name
        /// </summary>
        public string InputFilePath { get; set; }
        /// <summary>
        /// Gets or sets the station name
        /// </summary>
        public string StationName { get; set; }
    }
}
