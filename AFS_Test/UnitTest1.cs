using System;
using System.IO;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AFS_Test
{
    [TestClass]
    public class UnitTest1
    {
        string fileName = "testFile.txt";
        string streamName = "testStream";
        string testMessage = "Hello! I am a testing message! Привет! Я тестовое сообщение";
       
        private ApiDrive GetTestedObject()
        {
            return new ApiDrive();
        }

        [TestMethod]
        public void WriteMainTest() // Проверка записи в основной файловый поток
        {
            string CurrentPath = AppDomain.CurrentDomain.BaseDirectory + fileName;
            if (File.Exists(CurrentPath))
                File.Delete(CurrentPath);
            ApiDrive drive = GetTestedObject();
            byte[] outMessage = Encoding.Default.GetBytes(testMessage);
            byte[] inMessage;
            bool isEqual;
            try
            {
                using (FileStream fs = drive.OpenWithStream(CurrentPath, String.Empty, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.Write(outMessage, 0, outMessage.Length);
                }
                using (FileStream fs = drive.OpenWithStream(CurrentPath, String.Empty, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    inMessage = new byte[(int)fs.Length];
                    fs.Read(inMessage, 0, inMessage.Length);
                }
                isEqual = inMessage.SequenceEqual(outMessage);
                if (!isEqual)
                {
                    Assert.Fail("Записанное значение в основной поток не соответствует прочитанному: " + Encoding.Default.GetString(inMessage));
                }
                Assert.IsTrue(isEqual);
            }
            catch (Exception ex)
            {
                Assert.Fail("Ошибка чтения/записи в основной поток:\r\n" + ex.ToString());
            }
            finally
            {
                if (File.Exists(CurrentPath))
                    File.Delete(CurrentPath);
            }
        }

        [TestMethod]
        public void WriteAlterTest() // Проверка записи в алтернативный файловый поток
        {
            string CurrentPath = AppDomain.CurrentDomain.BaseDirectory + fileName;
            if (File.Exists(CurrentPath))
                File.Delete(CurrentPath);
            ApiDrive drive = GetTestedObject();
            byte[] outMessage = Encoding.Default.GetBytes(testMessage);
            byte[] inMessage;
            bool isEqual;
            try
            {
                using (FileStream fs = drive.OpenWithStream(CurrentPath, streamName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.Write(outMessage, 0, outMessage.Length);
                }
                using (FileStream fs = drive.OpenWithStream(CurrentPath, streamName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    inMessage = new byte[(int)fs.Length];
                    fs.Read(inMessage, 0, inMessage.Length);
                }
                isEqual = inMessage.SequenceEqual(outMessage);
                if (!isEqual)
                {
                    Assert.Fail("Записанное значение в альтернативный поток не соответствует прочитанному: " + Encoding.Default.GetString(inMessage));
                }
                Assert.IsTrue(isEqual);
            }
            catch (Exception ex)
            {
                Assert.Fail("Ошибка чтения/записи в альтернативный поток:\r\n" + ex.ToString());
            }
            finally
            {
                if (File.Exists(CurrentPath))
                    File.Delete(CurrentPath);
            }
        }
    }
}
