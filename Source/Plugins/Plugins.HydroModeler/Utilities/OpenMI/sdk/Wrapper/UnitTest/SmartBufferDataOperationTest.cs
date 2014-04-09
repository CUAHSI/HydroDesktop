using NUnit.Framework;

namespace Oatc.OpenMI.Sdk.Wrapper.UnitTest
{
  [TestFixture]
  public class SmartBufferDataOperationTest
  {
    private SmartBufferDataOperation _dataOperation;
    
    [SetUp]
    public void Setup()
    {
      _dataOperation = new SmartBufferDataOperation();
      _dataOperation.GetArgument(1).Value = "2";
    }

    [Test]
    public void Clone()
    {
      SmartBufferDataOperation dataOperation = (SmartBufferDataOperation)_dataOperation.Clone();
      Assert.AreEqual(_dataOperation.ArgumentCount, dataOperation.ArgumentCount, "ArgumentCount");

      Assert.AreEqual(_dataOperation.GetArgument(0).Description, dataOperation.GetArgument(0).Description, "Description 0");
      Assert.AreEqual(_dataOperation.GetArgument(0).Key, dataOperation.GetArgument(0).Key, "Key 0");
      Assert.AreEqual(_dataOperation.GetArgument(0).ReadOnly, dataOperation.GetArgument(0).ReadOnly, "ReadOnly 0");
      Assert.AreEqual(_dataOperation.GetArgument(0).Value, dataOperation.GetArgument(0).Value, "Value 0");

      Assert.AreEqual(_dataOperation.GetArgument(1).Description, dataOperation.GetArgument(1).Description, "Description 1");
      Assert.AreEqual(_dataOperation.GetArgument(1).Key, dataOperation.GetArgument(1).Key, "Key 1");
      Assert.AreEqual(_dataOperation.GetArgument(1).ReadOnly, dataOperation.GetArgument(1).ReadOnly, "ReadOnly 1");
      Assert.AreEqual(_dataOperation.GetArgument(1).Value, dataOperation.GetArgument(1).Value, "Value 1");

      Assert.AreEqual(_dataOperation.GetArgument(2).Description, dataOperation.GetArgument(2).Description, "Description 2");
      Assert.AreEqual(_dataOperation.GetArgument(2).Key, dataOperation.GetArgument(2).Key, "Key 2");
      Assert.AreEqual(_dataOperation.GetArgument(2).ReadOnly, dataOperation.GetArgument(2).ReadOnly, "ReadOnly 2");
      Assert.AreEqual(_dataOperation.GetArgument(2).Value, dataOperation.GetArgument(2).Value, "Value 2");      
    }
  }
}
