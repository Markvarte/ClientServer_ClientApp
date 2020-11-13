using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;


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


            ClientProgram Client = new ClientProgram();


            Thread ClientThread = new Thread(Client.ThreadStartClient);


            ClientThread.Start();
        }

    }
}
