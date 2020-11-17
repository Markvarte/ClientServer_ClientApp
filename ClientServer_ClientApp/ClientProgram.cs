using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.IO.MemoryMappedFiles;
public enum ClientProgramType { FIFO, queue, mappedMemory }

namespace ClientServer_ClientApp
{
    class ClientProgram
    {
        public void ThreadStartClient(object obj)
        {
            // Klients tiek startēts tikai pēc servera

            ManualResetEvent SyncClientServer = (ManualResetEvent)obj;

            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream("pipe"))
            {
                // Klienta puse gaidīs, kad tiks startēts serveris

                pipeStream.Connect();

                //Kad savienojums tiks izveidots, tiks atgriezta rindiņa par savienojuma izveidi

                Console.WriteLine("[Client] savienojums izveidots");
                //Nodod informāciju pa FIFO kanālu, notiek vienvirziena komunikācija, ar QUIT notiek iziešana no programmas (tiks aizvērti abi logi).

                using (StreamWriter sw = new StreamWriter(pipeStream))
                {
                    sw.AutoFlush = true;
                    string temp;
                    Console.WriteLine("Ierakstiet zinju un nospiediet [Enter], vai ierakstiet 'quit' lai izietu no programmas!");
                    while ((temp = Console.ReadLine()) != null)
                    {
                        if (temp == "quit") break;
                        sw.WriteLine(temp);
                    }
                }
            }
        }

        //Klients sūta ziņojumus, bet serveris saņem, tiek veidoti jauni pavedieni, kuri atgriež klienta informāciju


        static void Main(string[] args)
        {

            ClientProgramType c = ClientProgramType.mappedMemory; //  for now
            switch (c)
            {
                case ClientProgramType.FIFO: // 0
                    ClientProgram Client = new ClientProgram();


                    Thread ClientThread = new Thread(Client.ThreadStartClient);


                    ClientThread.Start();
                    break;


                case ClientProgramType.queue: // 1


                    break;

                case ClientProgramType.mappedMemory: // 2
                    //Atver un nolasa no servera rakstīto failu

                    Console.WriteLine("Memory mapped failu lasītājs");

                    using (var file = MemoryMappedFile.OpenExisting("myFile"))
                    {
                        using (var reader = file.CreateViewAccessor(0, 34))
                        {
                            var bytes = new byte[34];
                            reader.ReadArray<byte>(0, bytes, 0, bytes.Length);

                            Console.WriteLine("Lasa baitus");
                            for (var i = 0; i < bytes.Length; i++)
                                Console.Write((char)bytes[i] + " ");

                            Console.WriteLine(string.Empty);
                        }
                    }
                    Console.WriteLine("Nospiediet jebkuru taustiņu, lai izietu ...");
                    Console.ReadLine();

                    break;

                default:
                    Console.WriteLine("Wrong client type.");
                    break;
            }
                  
        }

    }
}
