using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ProcessCommunication
    {
        const int MMF_MAX_SIZE = 1024;  // allocated memory for this memory mapped file (bytes)
        const int MMF_VIEW_SIZE = 1024; // how many bytes of the allocated memory can this process access
        
        private static readonly string streamName = "sac1";

        private static int _serialNumber = 0;


        public void WriteStatusMessage(StatusValues status, string statusMessage)
        {
            // creates the memory mapped file which allows 'Reading' and 'Writing'
            MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen(streamName, MMF_MAX_SIZE, MemoryMappedFileAccess.ReadWrite);

            // creates a stream for this process, which allows it to write data from offset 0 to 1024 (whole memory)
            MemoryMappedViewStream mmvStream = mmf.CreateViewStream(0, MMF_VIEW_SIZE);

            // this is what we want to write to the memory mapped file
            Message msg = new Message();
            msg.Timestamp = DateTime.Now;
            msg.SerialNumber = ++_serialNumber;
            msg.StatusDescription = statusMessage;
            msg.Status = status;

            // serialize the variable 'message1' and write it to the memory mapped file
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(mmvStream, msg);
            mmvStream.Seek(0, SeekOrigin.Begin); // sets the current position back to the beginning of the stream
        }



        public Message ReadStatusMessage()
        {
            try
            {
                // creates the memory mapped file
                MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(streamName);
                MemoryMappedViewStream mmvStream = mmf.CreateViewStream(0, MMF_VIEW_SIZE); // stream used to read data
                if (mmvStream.CanRead)
                {
                    byte[] buffer = new byte[MMF_VIEW_SIZE];
                    Message msg = new Message();

                    // read the bytes from stream
                    mmvStream.Read(buffer, 0, MMF_VIEW_SIZE);

                    // deserializes the buffer
                    BinaryFormatter formatter = new BinaryFormatter();
                    msg = (Message)formatter.Deserialize(new MemoryStream(buffer));
                    //Console.WriteLine(mag.title + "\n" + mag.content + "\n");
                    return msg;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



    }
}
