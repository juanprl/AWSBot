using System.Globalization;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace hellowAWS2{
    
    class Program{
        
    static void Main(string[] args)
    {                      
            //0º Indicar día a buscar
            Console.WriteLine("Indica el mes y día de la carpeta a cambiar nombre. \n Ej: Para el día 28 de mayo, sería: 0428");
            string fechaTemp = Console.ReadLine();

            
            int time = DateTime.Now.Year;//Coge el año
            time = time - 2000;

            string year = time.ToString();
            string month = "";
            string day = "";

            try {
                
                if(fechaTemp.Length != 4 || int.Parse(fechaTemp) > 1300/*Has metido mal los datos. Ej: 2804 ha puesto el día primero*/){
                    //Si no cumple mis condiciones seguirá pidiendo meter los datos

                    while(fechaTemp.Length != 4 || int.Parse(fechaTemp) > 1300){ 
                        Console.WriteLine("Para el día 28 de mayo, sería: 0428 (Sin espacios)");
                        fechaTemp = Console.ReadLine();
                    }
                }

            }
            catch(FormatException e)
            {
                Console.Write("Caracteres no permitidos introducidos. Solo se admiten números \n");
                
                if(fechaTemp.Length != 4 || int.Parse(fechaTemp) > 1300/*Has metido mal los datos. Ej: 2804 ha puesto el día primero*/){
                    //Si no cumple mis condiciones seguirá pidiendo meter los datos

                    while(fechaTemp.Length != 4 || int.Parse(fechaTemp) > 1300){ 
                        Console.WriteLine("Para el día 28 de mayo, sería: 0428 (Sin espacios)");
                        fechaTemp = Console.ReadLine();
                    }
                }
            }

            char[] n = fechaTemp.ToArray();
            month += n[0];
            month += n[1];
            day += n[2];
            day += n[3];

            //1º - Descargar listado de los archivos json del día indicado en un txt que consultaremos adelante.

            string urlAWS = "s3://raasctipro-141965608953/connect/raasctipro00d7q000001pq2i/ContactEvaluations/20" 
            + year + "/" + month + "/" + day + "/";
            string command = "aws s3 ls " + urlAWS + " --recursive > listado.txt"; 

            //Console.Write("Prueba \n" + command);

            Process process = new Process();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = $"-Command \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            //2º Acceder al listado y borrar todos los caracteres inncesarios (Cuando descargamos )
            
            //2.1º Dividir en líneas según la terminación json 
            string dni = "80232126F";// # poner el dni de la persona o la palabra: usuario
            string filePath = @"C:\Users\" + dni + @"\Documents\BotsAWS\hellowAWS2\listado.txt"; 
            string fileContent = File.ReadAllText(filePath);

            //2.2º 
            //Dividir el txt en cadenas de strings, se puede cambiar como queremos dividir el texto solo escribiendo el texto deseado  

            List<String> nombresArchivos_list = new List<string>(); //Aquí guardamos los nombres de todos los archivos

            string[] nombresCadenas = fileContent.Split(".json"); 
            string temp = "";

            //Console.WriteLine(fileContent + " _Texto Descargado AWS \n"); //

            //2.2.Bº 
            //Pasar cadena de string a array y quitar, los caracteres iniciales que siempre son los mismos. 
            //Guardar en un List los que tengan : 

            for(int i = 0; i < nombresCadenas.Length - 1; i++){
                
                //Console.WriteLine(nombresCadenas[i] + " _línea original \n"); 
                
                temp = ""; //Reiniciamos variable
                char[] c = nombresCadenas[i].ToCharArray();

                //Console.WriteLine(c.Count() + " _longitud cadena \n"); 

                int numCaracteres = 95; //Por algún motivo que desconozco, la primera línea tiene menos caracteres
                if(i != 0)
                {
                    numCaracteres = 97;
                }
                

                for(int j = numCaracteres; j < c.Count(); j++){
                    temp += c[j].ToString();
                }

                nombresArchivos_list.Add(temp);

                //Console.WriteLine(temp + " _nombre archivo con : \n"); 
            }
           
            /*for(int i = 0; i < nombresArchivos_list.Count(); i++)//Ver que se lo han pasado bien.
            {              
                Console.WriteLine(nombresArchivos_list[i] + " _nombres de todos los archivo  \n");
            }*/

            //2.2.B2º 
            //Guardar SOLO en un List los que tengan ':' , si el programa ya se ha ejecutado anteriormente, ya habrá nombres sin :
            //Cambiar los : por _ del List

            List<String> nombresCambiar_list = new List<string>(); ///Aquí guardamos los nombres que van a ser remplazados y el nombre original(Hay que decir cual se va a cambiar).

            for(int i = 0; i < nombresArchivos_list.Count(); i++){
                
                if(nombresArchivos_list[i].Contains(":")){
                    nombresCambiar_list.Add(nombresArchivos_list[i] + ".json"); //Posición impar
                    nombresCambiar_list.Add(nombresArchivos_list[i].Replace(':','_') + ".json"); //Posición par
                    
                    //Console.WriteLine(nombresArchivos_list[i] + " _nombre original \n"); 
                    //Console.WriteLine(nombresArchivos_list[i].Replace(':','_') + " _nombre final \n"); 
                }
            }

            
            /*for(int i = 0; i < nombresCambiar_list.Count(); i++) //Ver que se lo han pasado bien.
            {              
                Console.WriteLine(nombresCambiar_list[i] + " _nombre a cambiar  \n");
            } */


            //3º Cambiar los nombres en AWS

            //3.1º Comando que le dice: archivoX: se llama ahora archivoX_
            
            for(int i = 0; i < nombresCambiar_list.Count(); i++)
            {              
                if(i % 2 == 0){
                    //Console.WriteLine(i + " _Prueba de salto de línea \n"); 
                    
                    command = "aws s3 mv " +
                    urlAWS + nombresCambiar_list[i] + " " + urlAWS + nombresCambiar_list[i + 1];  

                    //3.2º Hacer la operación
                    process = new Process();
                    process.StartInfo.FileName = "powershell.exe";
                    process.StartInfo.Arguments = $"-Command \"{command}\"";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();/**/

                    //Console.WriteLine(command + "\n _línea de comando nº " + i + " de " + nombresCambiar_list.Count() +" \n"); 
                    //Console.WriteLine(command + " #### " + "\n");
                    Console.WriteLine("Se han cambiado " + nombresCambiar_list[i + 1]);
                 }
            }            

            Console.WriteLine("Se han cambiado " + nombresCambiar_list.Count() / 2 + " archivos.");
        }
    }


}