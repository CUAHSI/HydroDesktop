using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using HydroDesktop.Configuration;
using System.Xml;

namespace HydroDesktop.Configuration.Tests
{
    [TestFixture]
    public class ConfigurationTest
    {
        //TODO need to rewrite these tests for the new handling of settings
        //ESpecially test the DataRepositoryConnectionString and CurrentProjectFileName.

        //[Test]
        //public void CanSaveSettings()
        //{
        //    Settings defaultSettings =  Settings.Instance;

        //    ConfigurationManager.SaveSettings(defaultSettings);
        //    Assert.That(File.Exists(ConfigurationManager.GetSettingsFileName()));

        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(ConfigurationManager.GetSettingsFileName());

        //    var hiscentral = doc.GetElementsByTagName("url");
        //       Assert.That(hiscentral.Count == 2);
        //       var recentproject = doc.GetElementsByTagName("RecentProject");

        //       Assert.That(recentproject.Count == 1);
        //       Assert.That(recentproject[0].Attributes["Path"].Value.Equals(@".\default.hdprj"));
        //    // future tests needed
        //}

        //[Test]
        //public void CanSaveLoadSettings()
        //{
        //    Settings settings1 = Settings.Instance;

        //     ConfigurationManager.SaveSettings(settings1);
 

        //    Settings settings2 = ConfigurationManager.LoadSettings();
        //    Assert.That(settings2.HISCentralURLList.Count == 2);

        //    Assert.AreEqual(settings1.DefaultHISCentralURL, settings2.DefaultHISCentralURL, "Default HISCentral urls should be equal");
        //    // have not changed it
        //    Assert.AreEqual(settings1.DefaultHISCentralURL, settings2.SelectedHISCentralURL, "selected HISCentral urls should be equal");
           
        //    string fakeurl = "http://example.com/hiscentral/webservices/hiscentral.asmx";
        //    settings1.HISCentralURLList.Add(fakeurl);
        //    settings1.SelectedHISCentralURL=fakeurl;
        //    settings1.RecentProject = @".\junk.hdpj";

        //    ConfigurationManager.SaveSettings(settings1);
        //    settings2.Load();
        //    Assert.That(settings2.HISCentralURLList.Count == 3, "after adding, count should be 3 his central url's");

        //    Assert.AreEqual(settings1.DefaultHISCentralURL, settings2.DefaultHISCentralURL, "Default HISCentral urls should be equal");
        //    // cahnged,saved, are they equal
        //    Assert.AreEqual(settings1.SelectedHISCentralURL, settings2.SelectedHISCentralURL, "selected his central was not updated on (re)load. selected HISCentral urls should be equal");
        //    Assert.AreEqual(settings1.RecentProject, settings2.RecentProject, "recent project was not updated on (re)load. ");
           
        //    // Assert.AreEqual(settings1.HISCentralURLList[1], settings2.HISCentralURLList[1], "the second optional urls should be equal");
  

        //}
    }
}
