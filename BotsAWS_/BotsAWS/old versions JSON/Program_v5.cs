/*
Empecemos, El paso a CSV debería hacerse de de otra manera, creando un objeto Formulario.cs donde se le pasa los datos del Json y luego el objeto a CSV,
tiempo que tarda en hacerse y memoria que usa no son optimos como lo hago ,
pero sino me da miles de errores si lo hago de lA MANERA CORRECTA, por eso lo hago así.
Soluciones: 
-Que lo haga el equipo de desarrollo en Java :)
*/

//ToDo: Si es 00 hacer todo el mes

using System.Text;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace jsonTo{
    
    class Program{
        
    static void Main(string[] args)
    {
            //0º Descargar los Json de AWS y crear carpeta si no existe
            
            //------------------------

            //0.1º Saber día y mes -- Por ahora manual, luego ya que lo saque del sistema
            Console.WriteLine("Indica el mes y día de la carpeta a cambiar nombre. \n Ej: Para el día 28 de mayo, sería: 0428");
            string fechaTemp = Console.ReadLine();//
            //string fechaTemp = "0530"; !!

            int time = DateTime.Now.Year;
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
                Console.Write("Caracteres no permitidos introducidos. Solo nº \n");
                
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


            //1º - Hacer del listado de archivos descargados

            //
            string dniUser = @"C:\Users\80232126F\Documents";

            //Tenemos que elegir una carpeta que solo tenga los JSON
            string folderPathJSON = dniUser + @"\BotsAWS\JSONS\Jsons\" + year + @"\" + month + @"\" + day + @"\"; 

            //Donde volcaremos los archivos terminados
            string folderPathCSV = dniUser + @"\BotsAWS\JSONS\Csv\" + year + @"\" + month + @"\" + day + @"\";

            //Donde guardamos una lista temporal solo necesaria para el programa
            string filePath = dniUser + @"\BotsAWS\jsonTo\listado.txt"; 
            string file2Path = dniUser + @"\BotsAWS\jsonTo\list2.txt"; 

            //Ruta del programa actual
            string programPath = dniUser + @"\BotsAWS\jsonTo\"; 

            //Ruta del programa hellowAWS2 para simplificar la ejecución para el usuario
            string hellowPath = dniUser + @"\BotsAWS\hellowAWS2\bin\Release\net7.0\win-x64\";         

           //!!Importante. Explicación tras ejecutar el CMD no se cerraba el proceso auqnue debería atascando el proceso.
           //Si lo pongo en un hilo se ejecuta y atasca el hilo pero no el programa. Después digo que tras un segundo continue el programa
           //ya que es un proceso súper sencillo en un segundo ya se habrá hecho cientos de veces.
            Thread hilo = new Thread(MiMetodo);
            hilo.Start();
            Thread.Sleep(1000);

            Thread hilo3 = new Thread(DescargaMasiva);
            hilo3.Start();
            Thread.Sleep(4000);
                       
            Thread hilo55 = new Thread(AddList);
            hilo55.Start();
            Thread.Sleep(9000);
                  
            //2º Acceder al listado y borrar todos los caracteres inncesarios
            
            //2.1º Dividir en líneas según la terminación json 
            string fileContent = File.ReadAllText(filePath);

            if(fileContent.Length < 5){
                Console.WriteLine("\n Error. No se ha descargado nada. Comprueba la conexión o si existen formularios de ese día");
            }

            //2.2.Aº Dividir el txt en cadenas de strings
            string[] nombresCadenas_temp = fileContent.Split(".json"); //Aquí se indica el tipo de archivo, se puede cambiar perfectamente solo escribiendo el tipo deseado
            
            //!!Por algún motivo si no lo hacemos así nos da un bug en la línea 117 ' temp = File.ReadAllText(pathTemp); ' 
            //donde no reconoce el string cómo debería
            List<string> nombresCadenas = new List<string>();
            nombresCadenas.Add(nombresCadenas_temp[0]);
            
            for(int i = 1; i < nombresCadenas_temp.Length - 1; i++){
                string[] nombres = nombresCadenas_temp[i].Split("\n");
                nombresCadenas.Add(nombres[1]);                
            }

            //--------------------------

            string temp = "";
            string pathTemp = "";

            //3 º Transformación
            for(int i = 0; i < nombresCadenas.Count; i++){
                pathTemp = folderPathJSON + nombresCadenas[i] + ".json";
                Console.WriteLine(" \n _Archivo a transformar  \n " + pathTemp); 
                temp = File.ReadAllText(pathTemp); //Reinicia //Accede al archivo JSON
                
                List<String> valoresCSV = new List<string>(); //Aquí guardamos la información que estará en el CSV

                //3.1º Identificar que tipo de archillo es
                if(temp.Contains("Monitorización I"))
                {
                    bool failed = false;
                    int cont = 0;
                    if(temp.Contains("\"automaticFail\":true}}")) {failed = true;}

                    temp = temp.Replace("\"","");//Quita los " de la copia JSON //Al hacerlo aquí ahorra tiempo de ejecución más adelante, en la parte de pasar los char
                    //Console.WriteLine(" \n _Paso transf 1º  \n " + temp); 

                    //3.2º Borramos texto no usable // Estamos borrando: "{"schemaVersion":"3.1","evaluationId":"1ce76893-6bf2-46f8-82c5-af6937086c03","metadata":{"           
                    char[] c = temp.ToCharArray();

                    temp = "";

                    for(int j = 89; j < c.Count(); j++){
                        temp += c[j].ToString();
                    }

                    //3.2º Dividimos el texto para poder sacar textos
                    string[] camposYvalores = temp.Split(",");

                    /*for(int j = 0; j < camposYvalores.Length - 1; j++)
                    {
                        Console.WriteLine(camposYvalores[j] + " \n _Paso transf 3º  \n "); 
                    }*/ 
                    
                    for(int j = 0; j < camposYvalores.Length - 1; j++)
                    {
                        if(j == 0) 
                        {
                            valoresCSV.Add(camposYvalores[0]);
                        }

                        if(j == 3 || j == 4 || j == 5 || j == 9) //Los diez primeros campos siguen la misma lógica
                        {
                            string[] valor = camposYvalores[j].Split(":");//Hacer un segundo Split con :                                                  
                            valoresCSV.Add(valor[1]); //Aquí debería estar el valor del campo
                        }

                        if(failed == false)
                        {
                            if(j == 10|| j == 13 || j == 16 || j == 19) //Se comporta distinto
                            {
                                string[] filo = camposYvalores[j].Split(":");

                                if( j == 19){
                                    string str = filo[2].Replace("}}]","");
                                    str = str.Replace(".",";");

                                    valoresCSV.Add(str);
                                }else{
                                    string str = filo[2].Replace("}}","");
                                    str = str.Replace(".",";");

                                    valoresCSV.Add(str);
                                }
                            } 
                        } 

                        if(failed == true && cont == 0) 
                        {
                            for(int jj = 0; jj < 4; jj++)
                            {
                                valoresCSV.Add("0");
                            }

                            cont++;
                        }             
                    }
                    
                  /*for(int j = 0; j < camposYvalores.Length - 1; j++)
                    {
                        Console.WriteLine(valoresCSV[j] + " \n _Paso transf 4º  \n "); 
                    } */

                    //3.2 Conversión, paso final
                    string valoresCSV_Final = "";
        
                    StringBuilder csvContent = new StringBuilder(); //!!Ponerlo fuera del bucle
                            
                    //Nombre de los campos
                    /*csvContent.AppendLine("ContactId,AgentId,Evaluation Definition Title,Evaluator,Evaluation Submit Timestamp,Score,"
                    +"Estructura de la interacción,Excelencia en la interacción,Aptitudes");*/
                            
                    valoresCSV_Final = valoresCSV[0] + "," + valoresCSV[1] + "," + valoresCSV[2] + "," + valoresCSV[3] + "," + 
                    valoresCSV[4] + "," + valoresCSV[5] + "," + valoresCSV[6] + "," + valoresCSV[7] + "," + valoresCSV[8];

                    //Console.WriteLine(valoresCSV_Final + " \n Campos a enviar csv, Monitorización \n "); 

                    csvContent.AppendLine(valoresCSV_Final); 

                    File.AppendAllText(folderPathCSV + "monitorizacion_" + fechaTemp + ".csv", csvContent.ToString());                   
                    //File.AppendAllText(folderPathCSV + nombresCadenas[i] + ".csv", csvContent.ToString());  
                }

                if(temp.Contains("Informe Calidad CERMAD RENFE"))
                {
                    bool failed = false;
                    int cont = 0;
                    if(temp.Contains("\"automaticFail\":true}}")) {failed = true;}

                    temp = temp.Replace("\"","");//Quita los " de la copia JSON //Al hacerlo aquí ahorra tiempo de ejecución más adelante, en la parte de pasar los char
                    //Console.WriteLine(" \n _Paso transf 1º  \n " + temp); 

                    //3.2º Dividimos el texto para poder sacar textos
                    string[] camposYvalores = temp.Split(",");

                    for(int j = 0; j < camposYvalores.Length - 122; j++)
                    {
                            if(j == 1 || j == 5 || j == 7 || j == 10 || j == 11 ) //Los diez primeros campos siguen la misma lógica
                            {
                                string[] valor = camposYvalores[j].Split(":");//Hacer un segundo Split con :                                                  
                                valoresCSV.Add(valor[1]); //Aquí debería estar el valor del campo
                            }

                            if(j == 2) 
                            {
                                string[] valor = camposYvalores[j].Split(":");
                                valoresCSV.Add(valor[2]); //Aquí debería estar el valor del campo
                            } 

                            if(failed == false)
                            {
                                if(j == 12 || j == 15 || j == 18 || j == 21 || j == 24 || j == 27) 
                                {
                                    string[] filo = camposYvalores[j].Split(":");

                                    string str = filo[2].Replace("}}","");
                                    str = str.Replace(".",";");

                                    if(j == 27)
                                    {
                                        str = str.Replace("]","");
                                    }

                                    valoresCSV.Add(str);
                                } 
                            } 

                        if(failed == true && cont == 0) 
                        {
                            for(int jj = 0; jj < 7; jj++)
                            {
                                valoresCSV.Add("0");
                            }

                            cont++;
                        }             
                    }
                    
                  /*for(int j = 0; j < camposYvalores.Length - 1; j++)
                    {
                        Console.WriteLine(valoresCSV[j] + " \n _Paso transf 4º  \n "); 
                    } */

                    //3.2 Conversión, paso final
                    string valoresCSV_Final = "";
        
                    StringBuilder csvContent = new StringBuilder(); //!!Ponerlo fuera del bucle
                            
                    //Nombre de los campos
                    //csvContent.AppendLine("Evaluation Id,Contact Id,Agent Id,Evaluator,Evaluation Start Timestamp,Evaluation Submit Timestamp,Score,BIENVENIDA / DESPEDIDA,SONDEO,RESOLUCIÓN,ACTITUD,COMUNICACIÓN");
                            
                    valoresCSV_Final = valoresCSV[0] + "," + valoresCSV[1] + "," + valoresCSV[2] + "," + valoresCSV[3] + "," + 
                    valoresCSV[4] + "," + valoresCSV[5] + "," + valoresCSV[6] + "," + valoresCSV[7] + "," + valoresCSV[8] + "," + valoresCSV[9] + "," + valoresCSV[10] + "," + valoresCSV[11];

                    //Console.WriteLine(valoresCSV_Final + " \n Campos a enviar csv, Monitorización \n "); 

                    csvContent.AppendLine(valoresCSV_Final); 

                    File.AppendAllText(folderPathCSV + "InformeCalidadCERMADRENFE_" + fechaTemp + ".csv", csvContent.ToString()); 
                    //File.AppendAllText(folderPathCSV + nombresCadenas[i] + ".csv", csvContent.ToString());*/  
                }                  
            }

            Environment.Exit(0); //Fuerza a que el programa termine

            //99º ¿Y si li subimos a OneDrive o algo para que todos accedan a ellos?
            void MiMetodo()
            {     
                //Crear carpeta si no existe, de ambos
                string command = "mkdir " + folderPathCSV + " 2>null";
                
                // Creamos un proceso para acceder a CMD
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.StandardInput.WriteLine(command);
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();

        //-------------
                command = "mkdir " + folderPathJSON + " 2>null";
                
                process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.StandardInput.WriteLine(command);
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
            }

            void DescargaMasiva(){
                string command = "aws s3 cp s3://raasctipro-141965608953/connect/raasctipro00d7q000001pq2i/ContactEvaluations/20" 
                + year + "/" + month + "/" + day + "/ " +  folderPathJSON + " --recursive  ";

                //Console.Write("Prueba \n" + command);

                Process process = new Process();
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = $"-Command \"{command}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }

            void AddList(){
                // Le pasa el listado de los archivos
                string command = "dir " + folderPathJSON + " /B > " + filePath + " ";

                //Console.Write("Prueba \n" + command);
                
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.StandardInput.WriteLine(command);
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                process.Dispose();
            }

            void ConvertirInforme_CERMAD(){
                
            }
        }
    }
}