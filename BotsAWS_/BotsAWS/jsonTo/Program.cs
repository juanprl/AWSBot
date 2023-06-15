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


            //1º - Hacer del listado de archivos descargados

            //
            string dniUser = @"C:\Users\80232126F\Documents"; //#Cambiar el Dni por el Dni del usuario o la palabra usuario para algunos cambios

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

            //IniciaEsto si vas a añdir un nuevo Formulario #
            //Ayudita();             

           //!!Importante. Explicación tras ejecutar el CMD no se cerraba el proceso auqnue debería atascando el proceso.
           //Si lo pongo en un hilo se ejecuta y atasca el hilo pero no el programa. Después digo que tras un segundo continue el programa
           //ya que es un proceso súper sencillo en un segundo ya se habrá hecho cientos de veces.
            Thread hilo = new Thread(CrearCarpetas);
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
            int contInforme = 0;

            //3 º Transformación
            for(int i = 0; i < nombresCadenas.Count; i++){
                pathTemp = folderPathJSON + nombresCadenas[i] + ".json";
                Console.WriteLine(" \n _Archivo a transformar  \n " + pathTemp); 
                temp = File.ReadAllText(pathTemp); //Reinicia //Accede al archivo JSON
                contInforme = i;
                
                //3.1º Identificar que tipo de archillo es

                //# Si añades otro tipo de informes, copia el if y pon el nombre del informe entre las comillas, ese nombre va dentro del json, pon solo el nombre sin carácteres especiales 
                if(temp.Contains("Informe Calidad CERMAD RENFE")) 
                {
                    ConvertirInforme_CERMAD(); //#Tendrás que crear un nuevo método con su nuevo nombre si añades otro formulario, puedes borrarlo si este formulario deja de existir
                }                   
            }

            Environment.Exit(0); //Fuerza a que el programa termine

            //99º ¿Y si li subimos a OneDrive o algo para que todos accedan a ellos?
            void CrearCarpetas()
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

                Console.Write("Proceso iniciado, descargando de S3 \n");

                Process process = new Process();
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = $"-Command \"{command}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }

            void AddList(){ //Esto dice qué archivos se van a transformar a CSV
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
                    
                    int number = 12; //# Nº de campos del CSV
                    
                    Console.WriteLine(" \n Conversión iniciada \n "); 

                    List<String> valoresCSV = new List<string>(); //Aquí guardamos la información que estará en el CSV
                    bool failed = false;
                    int cont = 0;
                    if(temp.Contains("\"automaticFail\":true}}")) {failed = true;}

                    temp = temp.Replace("\"","");//Quita los " de la copia JSON 

                    //3.2º Dividimos el texto para poder sacar textos
                    string[] camposYvalores = temp.Split(",");

                    //Si suspende varía bastante la estructura del formulario, respecto a mi código ya escrito, y es más limpio cambiar como divido el texto
                   //3.2.B.1º Corto el json original para solo quedarme con las secciones que guardan el nombre y la puntuación
                    string[] suspendidoTexto; 
                    string[] suspendidoTexto_2;

                    string[] camposYvalores_Suspendido =  new string[2];
                    
                    if(failed == true)
                    {
                        suspendidoTexto = temp.Split("},sections:[");
                        suspendidoTexto_2 = temp.Split("],questions:[");
                        camposYvalores_Suspendido = suspendidoTexto_2[0].Split("{sectionRefId:");
                    }

                    for(int j = 0; j < camposYvalores.Length - 1; j++)
                    {
                        //#
                        if(j == 1 || j == 5 || j == 7 || j == 10 || j == 11 ) //Los diez primeros campos siguen la misma lógica
                        {                            
                            string[] valor = camposYvalores[j].Split(":");//Hacer un segundo Split con :                                                  
                            valoresCSV.Add(valor[1]); //Aquí esta el valor del campo
                        }

                        if(j == 2) 
                        {
                            string[] valor = camposYvalores[j].Split(":");
                            valoresCSV.Add(valor[2]); //Aquí esta el valor del campo
                        }    
                  
                        if(failed == false) // :) Formulario Aprobado
                        {
                            //#
                            if(j == 12 ||j == 15 || j == 18 || j == 21 || j == 24 || j == 27) //Puntuaciones
                            {
                                string[] filo = camposYvalores[j].Split(":");

                                string str = filo[2].Replace("}}","");
                                str = str.Replace(".",",");
                                str = str.Replace("]","");

                                valoresCSV.Add(str);
                            } 
                        }                                          

                        //No tocar este if
                        if(valoresCSV.Count == number)
                        {
                            j = camposYvalores.Length - 2; //Esto hace que termine antes el for, tal vez sustituir por un break                          
                        } 
                    }

                    //Si hay un error fatal en el formulario varia bastante la estructura del formulario, hay que sacarlo del for y cosas
                    if(failed == true) // x_X Formulario  fallado 
                    {
                        //# Este es el Score general
                        valoresCSV.Add("INV");

                        for(int j = 1; j < camposYvalores_Suspendido.Length; j++)
                        {
                            //Console.WriteLine(" \n Sección: " + j + "\n " + camposYvalores_Suspendido[j]); 

                            if(camposYvalores_Suspendido[j].Contains("automaticFail:true}}"))
                            {
                                valoresCSV.Add("INV");
                            }else{
                                string[] filo = camposYvalores_Suspendido[j].Split(",");
                                string[] filo2 = filo[2].Split(":");

                                string str = filo2[2].Replace("}}","");
                                str = str.Replace(".",",");
                                str = str.Replace("]","");

                                valoresCSV.Add(str);
                            }                           
                        }
                    }

                    //3.2 Conversión, paso final
                    string valoresCSV_Final = "";
        
                    StringBuilder csvContent = new StringBuilder(); 

                    for(int j = 0; j < number; j++)
                    {
                        valoresCSV_Final += valoresCSV[j]; 

                        if(j != number - 1)
                        {
                            valoresCSV_Final += ";";
                        }
                    }
                   
                    csvContent.AppendLine(valoresCSV_Final); 

                    //# Tocar el " ", es el nombre del CSV resultante 
                    File.AppendAllText(folderPathCSV + "InformeCalidadCERMADRENFE_" + fechaTemp + ".csv", csvContent.ToString()); 
            } 

            void Ayudita()
            {
                //0.1º Saber día y mes -- Por ahora manual, luego ya que lo saque del sistema
                Console.WriteLine("Proceso Iniciado.\n Recuerda que este programa solo hace un informe a la vez, tendrás que cambiar de archivo manualmente \n ");

                //Tenemos que elegir una carpeta que tenga el JSON
                string filePathZ = dniUser + @"\BotsAWS\newF.json"; 

                //Guardamos el json en un string
                string fileContentZ = File.ReadAllText(filePathZ);

                //2.2.Aº Dividir el texto original en cadenas de strings
                string[] frasesZ = fileContentZ.Split(",");              
                
                int n = 50; //# poner nº que quieras, el objetivo es que acabe antes el json para que no te de tantas líneas
                for(int j = 0; j < frasesZ.Length - n; j++)
                {
                    Console.WriteLine(" \n" + " Línea: " + j +"  \n " + frasesZ[j]); 
                }     

                Environment.Exit(0); //Fuerza a que el programa termine
            }
        }
    }
}