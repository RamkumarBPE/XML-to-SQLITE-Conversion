#region Header Files
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Globalization;
#endregion

namespace LDOCE
{
    public static class Program
    {
        public static int totalTime;
        #region Elements Value Checking

        static string ElementValueNull(this XElement element)
        {
            if (element != null)
                return element.Value;

            return "";
        }
        #endregion

        #region Attributes Value Checking
        //This method is to handle if attribute is missing
        static string AttributeValueNull(this XElement element, string attributeName)
        {
            if (element == null)
                return "";
            else
            {
                XAttribute attr = element.Attribute(attributeName);
                return attr == null ? "" : attr.Value;
            }
        }
        #endregion

        #region Drop Table each time
        static void dropTable(string name)
        {
            SQLiteConnection cn = new SQLiteConnection("Data Source=LDOCE.sqlite");
            try
            {
                //var dbName = "LDOCE";
                string dropQuery = "DROP table IF EXISTS '" + name + "'";
                cn.Open();
                SQLiteCommand cm = new SQLiteCommand(dropQuery, cn);
                cm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem in Deleting Database:");
            }
            finally
            {
                cn.Close();
            }
        }
        #endregion

        #region INDEX Data
        public static void indexXML()
        {
            var tableName = "lookup";
            dropTable(tableName);
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE IF NOT EXISTS [lookup](
            [ID] VARCHAR NOT NULL ,
            [HWD] VARCHAR,
            [HOMNUM] VARCHAR,
            [POS] VARCHAR
            )";

            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            cmd1.ExecuteNonQuery();
            var xml = XElement.Load(@"XML\INDEX.xml");
            var q = from e in xml.Descendants("item")
                    select new
                    {
                        //TYPE = e.AttributeValueNull("type"),
                        ID = e.AttributeValueNull("id"),
                        //CLASS = e.AttributeValueNull("class"),
                        HWD = e.Element("hwd").ElementValueNull(),
                        HOMNUM = e.Element("homnum").ElementValueNull(),
                        POS = e.Element("pos").ElementValueNull()
                    };

            int count = 0;
            Stopwatch stp = new Stopwatch();
            stp.Start();
            var transaction = con.BeginTransaction();
            foreach (var k in q)
            {

                string insertQuery = String.Format("INSERT INTO lookup (id,hwd,homnum,pos) VALUES ( @id, @hwd, @homnum, @pos)");
                SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                cmd.CommandType = CommandType.Text;
                // cmd.Parameters.Add(new SQLiteParameter("@type", k.TYPE));
                cmd.Parameters.Add(new SQLiteParameter("@id", k.ID));
                //cmd.Parameters.Add(new SQLiteParameter("@class", k.CLASS));
                cmd.Parameters.Add(new SQLiteParameter("@hwd", k.HWD));
                cmd.Parameters.Add(new SQLiteParameter("@homnum", k.HOMNUM));
                cmd.Parameters.Add(new SQLiteParameter("@pos", k.POS));
                try
                {
                    //Console.WriteLine(count);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine(count);
                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(count);
                    throw new Exception(ex.Message);
                }
            }
            transaction.Commit();
            totalTime += count;
            Console.WriteLine("\n{0} records in {1} seconds.", count, stp.Elapsed.TotalSeconds);
            //Console.ReadLine();
            con.Close();
            Console.WriteLine(count);

        }
        #endregion

        #region Pictures Data
        public static void picsXML()
        {
            var tableName = "pics";
            dropTable(tableName);
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE IF NOT EXISTS [pics](
            [text] VARCHAR,
            [id] VARCHAR NOT NULL,
            [filename] VARCHAR
            )";

            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            cmd1.ExecuteNonQuery();
            var xml = XElement.Load(@"XML\PICTURES.xml");
            var q = from e in xml.Descendants("link")
                    select new
                    {

                        TEXT = e.Element("text").ElementValueNull(),
                        ID = e.Element("id").ElementValueNull(),
                        FILENAME = e.Element("filename").ElementValueNull()
                    };

            int count = 0;
            Stopwatch stp = new Stopwatch();
            stp.Start();
            var transaction = con.BeginTransaction();
            foreach (var k in q)
            {

                string insertQuery = String.Format("INSERT INTO pics (text,id,filename) VALUES ( @text,@id, @filename)");
                SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@text", k.TEXT));
                cmd.Parameters.Add(new SQLiteParameter("@id", k.ID));
                cmd.Parameters.Add(new SQLiteParameter("@filename", k.FILENAME));
                try
                {
                    Console.WriteLine(count);
                    cmd.ExecuteNonQuery();

                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(count);
                    throw new Exception(ex.Message);
                }
            }
            transaction.Commit();
            totalTime += count;
            Console.WriteLine("\n{0} records in {1} seconds.", count, stp.Elapsed.TotalSeconds);
            //Console.ReadLine();
            con.Close();
            Console.WriteLine(count);

        }
        #endregion

        #region Example Sound Data
        public static void exasoundXML()
        {
            var tableName = "exasound";
            dropTable(tableName);
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE IF NOT EXISTS [exasound](
            [text] VARCHAR,
            [id] VARCHAR NOT NULL PRIMARY KEY,
            [filename] VARCHAR
            )";

            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            cmd1.ExecuteNonQuery();
            var xml = XElement.Load(@"XML\EXAMPLE_SOUND.xml");
            var q = from e in xml.Descendants("link")
                    select new
                    {

                        TEXT = e.Element("text").ElementValueNull(),
                        ID = e.Element("id").ElementValueNull(),
                        FILENAME = e.Element("filename").ElementValueNull()
                    };

            int count = 0;
            Stopwatch stp = new Stopwatch();
            stp.Start();
            var transaction = con.BeginTransaction();
            foreach (var k in q)
            {

                string insertQuery = String.Format("INSERT INTO exasound (text,id,filename) VALUES ( @text,@id, @filename)");
                SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@text", k.TEXT));
                cmd.Parameters.Add(new SQLiteParameter("@id", k.ID));
                cmd.Parameters.Add(new SQLiteParameter("@filename", k.FILENAME));
                try
                {
                    Console.WriteLine(count);
                    cmd.ExecuteNonQuery();

                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(count);
                    throw new Exception(ex.Message);
                }
            }
            transaction.Commit();
            totalTime += count;
            Console.WriteLine("{0} records in {1} seconds.", count, stp.Elapsed.TotalSeconds);
            //Console.ReadLine();
            con.Close();
            Console.WriteLine(count);

        }
        #endregion

        #region US Sound Data
        public static void ussoundXML()
        {
            var tableName = "ussound";
            dropTable(tableName);
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE IF NOT EXISTS [ussound](
            [text] VARCHAR,
            [id] VARCHAR NOT NULL PRIMARY KEY,
            [filename] VARCHAR
            )";

            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            cmd1.ExecuteNonQuery();
            var xml = XElement.Load(@"XML\US_HWDSOUND.xml");
            var q = from e in xml.Descendants("link")
                    select new
                    {

                        TEXT = e.Element("text").ElementValueNull(),
                        ID = e.Element("id").ElementValueNull(),
                        FILENAME = e.Element("filename").ElementValueNull()
                    };

            int count = 0;
            Stopwatch stp = new Stopwatch();
            stp.Start();
            var transaction = con.BeginTransaction();
            foreach (var k in q)
            {

                string insertQuery = String.Format("INSERT INTO ussound (text,id,filename) VALUES ( @text,@id, @filename)");
                SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@text", k.TEXT));
                cmd.Parameters.Add(new SQLiteParameter("@id", k.ID));
                cmd.Parameters.Add(new SQLiteParameter("@filename", k.FILENAME));
                try
                {
                    Console.WriteLine(count);
                    cmd.ExecuteNonQuery();

                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(count);
                    throw new Exception(ex.Message);
                }
            }
            transaction.Commit();
            Console.WriteLine("{0} records in {1} seconds.", count, stp.Elapsed.TotalSeconds);
            //Console.ReadLine();
            totalTime += count;
            con.Close();

        }
        #endregion

        #region UK Headword Sound Data
        public static void uksoundXML()
        {
            var tableName = "uksound";
            dropTable(tableName);
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE IF NOT EXISTS [uksound](
            [text] VARCHAR,
            [id] VARCHAR NOT NULL PRIMARY KEY,
            [filename] VARCHAR
            )";

            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            cmd1.ExecuteNonQuery();
            var xml = XElement.Load(@"XML\UK_HWDSOUND.xml");
            var q = from e in xml.Descendants("link")
                    select new
                    {

                        TEXT = e.Element("text").ElementValueNull(),
                        ID = e.Element("id").ElementValueNull(),
                        FILENAME = e.Element("filename").ElementValueNull()
                    };

            int count = 0;
            Stopwatch stp = new Stopwatch();
            stp.Start();
            var transaction = con.BeginTransaction();
            foreach (var k in q)
            {

                string insertQuery = String.Format("INSERT INTO uksound (text,id,filename) VALUES ( @text,@id, @filename)");
                SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@text", k.TEXT));
                cmd.Parameters.Add(new SQLiteParameter("@id", k.ID));
                cmd.Parameters.Add(new SQLiteParameter("@filename", k.FILENAME));
                try
                {
                    Console.WriteLine(count);
                    cmd.ExecuteNonQuery();

                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(count);
                    throw new Exception(ex.Message);
                }
            }
            transaction.Commit();
            Console.WriteLine("{0} records in {1} seconds.", count, stp.Elapsed.TotalSeconds);
            //Console.ReadLine();
            totalTime += count;
            con.Close();

        }
        #endregion

        #region Wordsets Links Data
        public static void wordsets_linksXML()
        {
            var tableName = "wordsets_links";
            dropTable(tableName);
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE IF NOT EXISTS [wordsets_links](
            [text] VARCHAR,
            [id] VARCHAR NOT NULL,
            [key] VARCHAR
            )";

            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            cmd1.ExecuteNonQuery();
            var xml = XElement.Load(@"XML\WORDSETS_LINKS.xml");
            var q = from e in xml.Descendants("link")
                    select new
                    {

                        TEXT = e.Element("text").ElementValueNull(),
                        ID = e.Element("id").ElementValueNull(),
                        FILENAME = e.Element("key").ElementValueNull()
                    };

            int count = 0;
            Stopwatch stp = new Stopwatch();
            stp.Start();
            var transaction = con.BeginTransaction();
            foreach (var k in q)
            {

                string insertQuery = String.Format("INSERT INTO wordsets_links (text,id,key) VALUES ( @text,@id, @key)");
                SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@text", k.TEXT));
                cmd.Parameters.Add(new SQLiteParameter("@id", k.ID));
                cmd.Parameters.Add(new SQLiteParameter("@key", k.FILENAME));
                try
                {
                    Console.WriteLine(count);
                    cmd.ExecuteNonQuery();

                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(count);
                    throw new Exception(ex.Message);
                }
            }
            transaction.Commit();
            Console.WriteLine("{0} records in {1} seconds.", count, stp.Elapsed.TotalSeconds);
            //Console.ReadLine();
            con.Close();
            totalTime += count;
        }
        #endregion

        #region VERBTABLES LINK DATA
        public static void verbtables_linkXML()
        {
            var tableName = "verbtables_link";
            dropTable(tableName);
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE IF NOT EXISTS [verbtables_link](
            
            [id] VARCHAR NOT NULL PRIMARY KEY,
            [key] VARCHAR
            
            )";

            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            cmd1.ExecuteNonQuery();
            var xml = XElement.Load(@"XML\VERBTABLES_LINKS.xml");
            var q = from e in xml.Descendants("link")
                    select new
                    {


                        ID = e.Element("id").ElementValueNull(),
                        KEY = e.Element("key").ElementValueNull()
                    };

            int count = 0;
            Stopwatch stp = new Stopwatch();
            stp.Start();
            var transaction = con.BeginTransaction();
            foreach (var k in q)
            {

                string insertQuery = String.Format("INSERT INTO verbtables_link (id,key) VALUES (@id, @key)");
                SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(new SQLiteParameter("@id", k.ID));
                cmd.Parameters.Add(new SQLiteParameter("@key", k.KEY));
                try
                {
                    Console.WriteLine(count);
                    cmd.ExecuteNonQuery();

                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(count);
                    throw new Exception(ex.Message);
                }
            }
            transaction.Commit();
            Console.WriteLine("{0} records in {1} seconds", count, stp.Elapsed.TotalSeconds);
            //Console.ReadLine();
            totalTime += count;
            con.Close();

        }
        #endregion

        #region ETYMOLOGY LINKS Data
        public static void etymology_linksXML()
        {
            var tableName = "etymology_links";
            dropTable(tableName);
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE IF NOT EXISTS [etymology_links](
            [text] VARCHAR,
            [id] VARCHAR NOT NULL PRIMARY KEY,
            [key] VARCHAR
            )";

            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            cmd1.ExecuteNonQuery();
            var xml = XElement.Load(@"XML\ETYMOLOGY_LINKS.xml");
            var q = from e in xml.Descendants("link")
                    select new
                    {

                        TEXT = e.Element("text").ElementValueNull(),
                        ID = e.Element("id").ElementValueNull(),
                        KEY = e.Element("key").ElementValueNull()
                    };

            int count = 0;
            Stopwatch stp = new Stopwatch();
            stp.Start();
            var transaction = con.BeginTransaction();
            foreach (var k in q)
            {

                string insertQuery = String.Format("INSERT INTO etymology_links (text,id,key) VALUES ( @text,@id, @key)");
                SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@text", k.TEXT));
                cmd.Parameters.Add(new SQLiteParameter("@id", k.ID));
                cmd.Parameters.Add(new SQLiteParameter("@key", k.KEY));
                try
                {
                    Console.WriteLine(count);
                    cmd.ExecuteNonQuery();

                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(count);
                    throw new Exception(ex.Message);
                }
            }
            transaction.Commit();
            Console.WriteLine("{0} records in {1} seconds", count, stp.Elapsed.TotalSeconds);
            //Console.ReadLine();
            totalTime += count;
            con.Close();

        }
        #endregion

        #region VerbTables Data
        public static void verbtablesXML()
        {
            var tableName = "verbtables";
            dropTable(tableName);
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE IF NOT EXISTS [verbtables](
            [id] VARCHAR NOT NULL PRIMARY KEY,
            [data] TEXT
            )";

            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            cmd1.ExecuteNonQuery();
            XElement xml = XElement.Load(@"xml\VERBTABLES.xml",LoadOptions.PreserveWhitespace);
            var q = from e in xml.Descendants("Entry")
                    select new
                    {
                        ID = e.AttributeValueNull("id")
                    };
            int count = 0;
            Stopwatch stop = new Stopwatch();
            stop.Start();
            var transaction = con.BeginTransaction();
            foreach (var disp in q)
            {
                //XElement main = XElement.Load(@"xml\VERBTABLES.xml");
                IEnumerable<XElement> searched =
                    from c in xml.Elements("Entry")
                    where (string)c.Attribute("id") == disp.ID
                    select c;
                foreach (var baz in searched)
                {
                    XElement min = baz.Element("div");
                    string insertQuery = String.Format("INSERT INTO verbtables (id,data) VALUES (@id, @data)");
                    SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SQLiteParameter("@id", disp.ID));
                    cmd.Parameters.Add(new SQLiteParameter("@data", min.ToString()));
                    cmd.ExecuteNonQuery();
                    count++;
                }
            }
            transaction.Commit();
            Console.WriteLine("{0} records in {1} seconds", count, stop.Elapsed.TotalSeconds);
            totalTime += count;
            stop.Stop();
            con.Close();

        }
        #endregion

        #region WORD SETS Data
        public static void wordsetsXML()
        {
            var tableName = "wordsets";
            //dropTable(tableName);
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE IF NOT EXISTS [wordsets](
            [id] VARCHAR,
            [name] VARCHAR,
            [refid] VARCHAR,
            [hwd] VARCHAR,
            [pos] VARCHAR
            )";

            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            cmd1.ExecuteNonQuery();
            //Console.WriteLine("entereddd");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"XML\WORDSETS.xml");
            XmlElement root = xmlDoc.DocumentElement;
            XmlNodeList link = root.SelectNodes("category");
            int cnt = 0;
            Stopwatch stp = new Stopwatch();
            stp.Start();
            var transaction = con.BeginTransaction();

            foreach (XmlElement node in link)
            {
                String id = node.GetAttribute("id");
                XmlNode wordhead = node.FirstChild;
                XmlNode Names = wordhead.FirstChild;
                String Name = Names.InnerText;
                XmlNode wsbody = wordhead.NextSibling;
                XmlElement child = (XmlElement)wsbody.FirstChild;

                while (wsbody.LastChild != child)
                {
                    XmlElement reff = child;
                    String refid = reff.GetAttribute("refid");
                    XmlElement hwd = (XmlElement)reff.FirstChild;
                    String HWD = hwd.InnerText;
                    hwd = (XmlElement)hwd.NextSibling;
                    String pos = hwd.InnerText;
                    child = (XmlElement)child.NextSibling;


                    string insertQuery = String.Format("INSERT INTO wordsets (id,name,refid,hwd,pos) VALUES (@id,@name,@refid,@hwd, @pos)");
                    SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SQLiteParameter("@id", id));
                    cmd.Parameters.Add(new SQLiteParameter("@name", Name));
                    cmd.Parameters.Add(new SQLiteParameter("@refid", refid));
                    cmd.Parameters.Add(new SQLiteParameter("@hwd", HWD));
                    cmd.Parameters.Add(new SQLiteParameter("@pos", pos));
                    try
                    {
                        Console.WriteLine(cnt);
                        cmd.ExecuteNonQuery();
                        cnt++;
                        //count++;
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(count);
                        throw new Exception(ex.Message);
                    }




                }
            }
            transaction.Commit();
            stp.Stop();
            totalTime += cnt;
            Console.WriteLine("{0} records in {1} seconds", cnt, stp.Elapsed.TotalSeconds);
            con.Close();

        }
        #endregion

        /*  #region Etymologies Data
        public static void EtymologiesXML()
           {
               var tableName = "Etymologies";
               dropTable(tableName);
               SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
               con.Open();
               string createTable = @" CREATE TABLE IF NOT EXISTS [Etymologies](
            [ID] VARCHAR NOT NULL,
            [HWD] VARCHAR,
            [HOMNUM] VARCHAR,
            [NUM] VARCHAR,
            [LANG] VARCHAR,
            [CENTURY] VARCHAR,[ORIGIN] VARCHAR,[ORIGIN_REFHWD] VARCHAR,[ORIGIN_REFHOM] VARCHAR,[ORIGIN_REFSENSE] VARCHAR,[Z] VARCHAR,[TRAN] VARCHAR,[TOPIC] VARCHAR,[REFHWD] VARCHAR, 
            [REFHOM] VARCHAR,[REFSENSE] VARCHAR,[D] VARCHAR,[L] VARCHAR,[O] VARCHAR
            )";
               SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
               cmd1.ExecuteNonQuery();
               var xml = XElement.Load(@"xml\ETYMOLOGIES.xml");
               var ggg = xml.Element("Ref").ElementValueNull();
               var q = from e in xml.Descendants("Entry")
                       select new
                       {
                           ID = e.AttributeValueNull("id"),
                           HWD=e.Element("HEAD").Element("HWD").ElementValueNull(),
                           HOMNUM=e.Element("HEAD").Element("HOMNUM").ElementValueNull(),
                           NUM=e.Element("SENSE").AttributeValueNull("num"),
                           LANG=e.Element("SENSE").Element("LANG").ElementValueNull(),
                           CENTURY=e.Element("SENSE").Element("CENTURY").ElementValueNull(),
                           ORIGIN=e.Element("SENSE").Element("ORIGIN").ElementValueNull(),
                           ORIGIN_REFHWD=e.Element("SENSE").Element("ORIGIN").Element("REFHWD").ElementValueNull(),
                           ORIGIN_REFHOM = e.Element("SENSE").Element("ORIGIN").Element("REFHOM").ElementValueNull(),
                           ORIGIN_REFSENSE = e.Element("SENSE").Element("ORIGIN").Element("REFSENSE").ElementValueNull(),
                           Z = e.Element("SENSE").Element("Z").ElementValueNull(),
                           TRAN = e.Element("SENSE").Element("TRAN").ElementValueNull(),
                           TOPIC = e.Element("SENSE").Element("Ref").AttributeValueNull("topic"),
                           REFHWD = e.Element("REFHWD").ElementValueNull(),
                           REFHOM = e.Element("REFHOM").ElementValueNull(),
                           REFSENSE = e.Element("REFSENSE").ElementValueNull(),
                           D = e.Element("SENSE").Element("D").ElementValueNull(),
                           L = e.Element("SENSE").Element("L").ElementValueNull(),
                           O = e.Element("SENSE").Element("O").ElementValueNull()

                       };
               int count = 0;
               Stopwatch stp = new Stopwatch();
               stp.Start();
               var transaction = con.BeginTransaction();
               foreach (var k in q)
               {

                   string insertQuery = String.Format("INSERT INTO Etymologies (id,hwd,homnum,num,lang,century,origin,origin_refhwd,origin_refhom,origin_refsense,z,tran,topic,refhwd,refhom,refsense,d,l,o) VALUES ( @id, @hwd, @homnum, @num,@lang,@century,@origin,@origin_refhwd,@origin_refhom,@origin_refsense,@z,@tran,@topic,@refhwd,@refhom,@refsense,@d,@l,@o)");
                   SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                   cmd.CommandType = CommandType.Text;
                   cmd.Parameters.Add(new SQLiteParameter("@id", k.ID));
                   cmd.Parameters.Add(new SQLiteParameter("@hwd", k.HWD));
                   cmd.Parameters.Add(new SQLiteParameter("@homnum", k.HOMNUM));
                   cmd.Parameters.Add(new SQLiteParameter("@num", k.NUM));
                   cmd.Parameters.Add(new SQLiteParameter("@lang", k.LANG));
                   cmd.Parameters.Add(new SQLiteParameter("@century", k.CENTURY));
                   cmd.Parameters.Add(new SQLiteParameter("@origin", k.ORIGIN));
                   cmd.Parameters.Add(new SQLiteParameter("@origin_refhwd", k.ORIGIN_REFHWD));
                   cmd.Parameters.Add(new SQLiteParameter("@origin_refhom", k.ORIGIN_REFHOM));
                   cmd.Parameters.Add(new SQLiteParameter("@origin_refsense", k.ORIGIN_REFSENSE));
                   cmd.Parameters.Add(new SQLiteParameter("@z", k.Z));
                   cmd.Parameters.Add(new SQLiteParameter("@tran", k.TRAN));
                   cmd.Parameters.Add(new SQLiteParameter("@topic", k.TOPIC));
                   cmd.Parameters.Add(new SQLiteParameter("@refhwd", k.REFHWD));
                   cmd.Parameters.Add(new SQLiteParameter("@refhom", k.REFHOM));
                   cmd.Parameters.Add(new SQLiteParameter("@refsense", k.REFSENSE));
                   cmd.Parameters.Add(new SQLiteParameter("@d", k.D));
                   cmd.Parameters.Add(new SQLiteParameter("@l", k.L));
                   cmd.Parameters.Add(new SQLiteParameter("@o", k.O));
                   try
                   {
                       Console.WriteLine(count);
                       cmd.ExecuteNonQuery();

                       count++;
                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine(count);
                       throw new Exception(ex.Message);
                   }
               }
               transaction.Commit();
               Console.WriteLine("{0} seconds with one transaction.", stp.Elapsed.TotalSeconds);
               //Console.ReadLine();
               con.Close();
               Console.WriteLine(count);
               
           }
        #endregion*/

        #region ETYMOLOGY Data
        public static void etymologyXML()
        {
            var tableName = "etymology";
            dropTable(tableName);
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE IF NOT EXISTS [etymology](
            [id] VARCHAR PRIMARY KEY NOT NULL,
            [hwd] VARCHAR,
            [sense] VARCHAR
            )";

            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            cmd1.ExecuteNonQuery();
            //Console.WriteLine("entereddd");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(@"XML\ETYMOLOGY.xml");
            XmlElement root = xmlDoc.DocumentElement;
            XmlNodeList link = root.SelectNodes("Entry");
            int count = 0;
            Stopwatch stp = new Stopwatch();
            stp.Start();
            var transaction = con.BeginTransaction();
            foreach (XmlElement node in link)
            {
                String id = node.GetAttribute("id");
                XmlNode head = node.FirstChild;
                XmlNode Names = head.FirstChild;
                String hwd = Names.InnerText;
                XmlNode sense = head.NextSibling;
                String Sens = "";

                var senses = node.GetElementsByTagName("SENSE");
                foreach (XmlNode s in senses)
                {
                    Sens += s.OuterXml.ToString();
                }



                string insertQuery = String.Format("INSERT INTO Etymology (id,hwd,sense) VALUES (@id,@hwd,@sense)");
                SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@id", id));
                cmd.Parameters.Add(new SQLiteParameter("@hwd", hwd));
                cmd.Parameters.Add(new SQLiteParameter("@sense", Sens));

                try
                {
                    count++;
                    Console.WriteLine(count);
                    cmd.ExecuteNonQuery();


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message);
                }


            }
            transaction.Commit();
            stp.Stop();
            totalTime += count;
            Console.WriteLine("{0} entries in {1} seconds.", count, stp.Elapsed.TotalSeconds);
            con.Close();


        }
        #endregion

        #region LDOCE6 Data
        public static void coreXML()
        {

            var tableName = "core";
            dropTable(tableName);
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE IF NOT EXISTS [core](
            
            [id] VARCHAR NOT NULL PRIMARY KEY,
            [type] VARCHAR,
            [new] VARCHAR,
            [src] VARCHAR,
            [sortname] VARCHAR,
            [online_only] VARCHAR,
            [HWD] VARCHAR,
            [PHRVBHWD] VARCHAR,
            [HYPHENATION] VARCHAR,
            [HOMNUM] VARCHAR,
            [ADDITIONAL_INFO] VARCHAR,
            [SEM] VARCHAR,
            [SearchInflections] VARCHAR,
            [SENSES] VARCHAR,
            [SPOKEN_SECTS] VARCHAR,
            [TAILS] VARCHAR,
            [PHRVBENTRIES] VARCHAR
            
            )";

            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            cmd1.ExecuteNonQuery();


            XElement xml = XElement.Load(@"xml\core.xml",LoadOptions.PreserveWhitespace);
            var q = from e in xml.Descendants("Entry")
                    select new
                    {
                        SORTNAME = e.AttributeValueNull("sortname"),
                        ID = e.AttributeValueNull("id"),
                        TYPE = e.AttributeValueNull("type"),
                        NEW = e.AttributeValueNull("new"),
                        SRC = e.AttributeValueNull("src"),
                        ONLINE_ONLY = e.AttributeValueNull("online_only"),
                        HWD = e.Element("Head").Element("HWD").ElementValueNull(),
                        PHRVBHWD = e.Element("Head").Element("PHRVBHWD").ElementValueNull(),
                        HYPHENATION = e.Element("Head").Element("HYPHENATION").ElementValueNull(),
                        HOMNUM = e.Element("Head").Element("HOMNUM").ElementValueNull(),
                        ADDITIONAL_INFO = e.Elements("Head") == null ? "" :getAdditionalInfo(cleanXMLByRemovingNewLinesAndTabs(e.Element("Head").ToString(SaveOptions.DisableFormatting))),
                        SEM = e.Element("Head").Element("SEM").ElementValueNull(),
                        SearchInflections = e.Element("Head").Element("SearchInflections") == null ? "" : NestedToString(e.Element("Head").Element("SearchInflections").Elements("WF")),
                        SENSES = e.Elements("Entry") == null ? "" : applyRegex(e),
                        //SENSES = e.Element("Entry").Elements("Sense") == null ? (e.Element("Entry").Elements("SpokenSect")) == null ? "" : NestedToString(e.Element("Entry").Elements("SpokenSect")) : NestedToString(e.Element("Entry").Elements("Sense")),
                        SpokenSects = e.Element("SpokenSect") == null ? "" : NestedToString(e.Element("SpokenSect").Elements("Sense")),
                        //SpokenSects = e.Element("SpokenSect") == null? "": NestedToString(e.Elements("SpokenSect")),
                        Tails = e.Elements("Tail") == null ? "" : NestedToString(e.Elements("Tail")),
                        PhrVbEntries = e.Elements("PhrVbEntry") == null ? "" : NestedToString(e.Elements("PhrVbEntry"))

                    };



            /*------------------------
             * 
             * 
             * 
             * 
             * ------------------------------------------------------------------------------------------------------*/





            /** Test code **/
            //var q = xml.Descendants("Entry");
            //foreach (var non in q)
            //{
            //    Console.WriteLine(SensesToString(non.Elements("Sense")));
            //    //Console.WriteLine(non.SENSES);
            //}


            int count = 0;
            Stopwatch stp = new Stopwatch();
            stp.Start();
            var transaction = con.BeginTransaction();
            foreach (var k in q)
            {

                string insertQuery = String.Format("INSERT INTO CORE (sortname,id,type,new,src,online_only,HWD,PHRVBHWD,HYPHENATION,HOMNUM,ADDITIONAL_INFO,SEM,SearchInflections,SENSES,SPOKEN_SECTS,TAILS,PHRVBENTRIES) VALUES" +
                                                                    "(@sortname,@id,@type,@new,@src,@online_only,@HWD,@PHRVBHWD,@HYPHENATION,@HOMNUM,@ADDITIONAL_INFO,@SEM,@SearchInflections,@SENSES,@SPOKEN_SECTS,@TAILS,@PHRVBENTRIES)");
                SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(new SQLiteParameter("@sortname", k.SORTNAME));
                cmd.Parameters.Add(new SQLiteParameter("@id", k.ID));
                cmd.Parameters.Add(new SQLiteParameter("@type", k.TYPE));
                cmd.Parameters.Add(new SQLiteParameter("@new", k.NEW));
                cmd.Parameters.Add(new SQLiteParameter("@src", k.SRC));
                cmd.Parameters.Add(new SQLiteParameter("@online_only", k.ONLINE_ONLY));
                cmd.Parameters.Add(new SQLiteParameter("@HWD", k.HWD));
                cmd.Parameters.Add(new SQLiteParameter("@PHRVBHWD", k.PHRVBHWD));
                cmd.Parameters.Add(new SQLiteParameter("@HYPHENATION", k.HYPHENATION));
                cmd.Parameters.Add(new SQLiteParameter("@HOMNUM", k.HOMNUM));
                cmd.Parameters.Add(new SQLiteParameter("@ADDITIONAL_INFO", k.ADDITIONAL_INFO));
                cmd.Parameters.Add(new SQLiteParameter("@SEM", k.SEM));
                cmd.Parameters.Add(new SQLiteParameter("@SearchInflections", k.SearchInflections));
                cmd.Parameters.Add(new SQLiteParameter("@SENSES", k.SENSES));
                cmd.Parameters.Add(new SQLiteParameter("@SPOKEN_SECTS", k.SpokenSects));
                cmd.Parameters.Add(new SQLiteParameter("@TAILS", k.Tails));
                cmd.Parameters.Add(new SQLiteParameter("@PHRVBENTRIES", k.PhrVbEntries));

                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine(count);
                    count++;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            transaction.Commit();
            stp.Stop();
            totalTime += count;
            Console.WriteLine("{0} entries in {1} seconds.", count, stp.Elapsed.TotalSeconds);
            //Console.ReadLine();
            con.Close();


        }



        #region SearchInflections Table
        public static void coreSearchInflections()
        {
            var tableName = "searchinflections";
            dropTable(tableName);
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE IF NOT EXISTS [searchinflections](
            [hwd] VARCHAR,
            [InflectionWords] VARCHAR,
            UNIQUE (hwd,InflectionWords) 
            )";

            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            cmd1.ExecuteNonQuery();
            XElement xml = XElement.Load(@"xml\core.xml");
            var q = from e in xml.Descendants("Entry")
                    select new
                    {
                        HWD = e.Element("Head").Element("HWD").ElementValueNull(),
                        SearchInflections = e.Element("Head").Element("SearchInflections") == null ? "" : NestedToString(e.Element("Head").Element("SearchInflections").Elements("WF"))
                    };
            int count = 0;
            Stopwatch stp = new Stopwatch();
            stp.Start();
            var transaction = con.BeginTransaction();

            foreach (var s in q)
            {
                if (s.SearchInflections == "")
                {
                }
                else
                {
                    var hwd = s.HWD;
                    //string search = "<WF>searches</WF><WF>searched</WF><WF></WF>";
                    string search = s.SearchInflections;
                    string[] inflections = search.Split(new String[] { "<WF>", "</WF><WF>", "</WF>" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string inflection in inflections)
                    {
                        string insertQuery = String.Format("INSERT INTO searchinflections (hwd, InflectionWords) VALUES" + " (@hwd,@inflection)");
                        SQLiteCommand cmd = new SQLiteCommand(insertQuery, con);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SQLiteParameter("@hwd", hwd));
                        cmd.Parameters.Add(new SQLiteParameter("@inflection", inflection));
                        Console.WriteLine(inflection);

                        try
                        {
                            cmd.ExecuteNonQuery();
                            Console.WriteLine(count);
                            count++;
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception(ex.Message);
                        }
                    }

                }
            }
            transaction.Commit();
            stp.Stop();
            totalTime += count;
            Console.WriteLine("{0} entries in {1} seconds.", count, stp.Elapsed.TotalSeconds);
            //Console.ReadLine();
            con.Close();

        }
        #endregion



        private static string NestedToString(IEnumerable<XElement> elements)
        {
            string result = string.Empty;
            foreach (var element in elements)
            {
                result += element.ToString(SaveOptions.DisableFormatting);
            }
            result = cleanXMLByRemovingNewLinesAndTabs(result);
            return result;
        }

        private static string applyRegex(XElement xml)
        {
            string result = string.Empty;
            String resultString = xml.ToString(SaveOptions.DisableFormatting);
            resultString = filterSelfEnclosedTag(resultString);
        
            //Regex regexForSenses = new Regex(@"<Sense.*?</Sense>|<SpokenSect.*?</SpokenSect>");
            MatchCollection matches = Regex.Matches(resultString, @"<Sense(.|\n)*?<\/Sense>|<SpokenSect(.|\n)*?<\/SpokenSect>|<PhrVbEntry(.|\n)*?<\/PhrVbEntry>");
            foreach (Match match in matches)
            {
                result += match.Value;
            }
           // resultString = filterSelfEnclosedTag(resultString);
            result = Regex.Replace(result, "<PhrVbEntry.*?</PhrVbEntry>", "");
           // Console.WriteLine(resultString);
            result = cleanXMLByRemovingNewLinesAndTabs(result);
            return result;
        }

        private static string cleanXMLByRemovingNewLinesAndTabs(String xml)
        {
            String resultString = xml;
            resultString = resultString.Replace(System.Environment.NewLine, "");
            resultString = Regex.Replace(resultString, "[\t]{1,}", "");
            return resultString;
        }

        //filterSelfEnclosedTag
        private static string filterSelfEnclosedTag(string xml)
        {
            String resultString = Regex.Replace(xml, "<Sense.[^>]*?/>", "");
            return resultString;
        }

        private static string getAdditionalInfo(string xml)
        {
            string result = string.Empty;
            //Removing the HWD, HOMNUM, SearchInflections and HYPHENATION tags as we have separate columns for them.
            result = Regex.Replace(xml,"<HWD.*?</HWD>|<HOMNUM.*?</HOMNUM>|<HYPHENATION.*?</HYPHENATION>|<SearchInflections.*?</SearchInflections>","");
            //Removing the enclosing tags
            result = Regex.Replace(result, "<Head>|</Head>", "");
            return result;
        }
        #endregion

        #region createLookupfromcore
        private static void createLookupFromCore()
        {
            SQLiteConnection con = new SQLiteConnection("Data Source=LDOCE.sqlite");
            con.Open();
            string createTable = @" CREATE TABLE LOOKUP AS SELECT ID,HWD,HOMNUM FROM core";
            SQLiteCommand cmd1 = new SQLiteCommand(createTable, con);
            int success= cmd1.ExecuteNonQuery();
            String qry = @"select hwd,id from lookup";
            SQLiteCommand command = new SQLiteCommand(qry, con);
            SQLiteDataReader reader = command.ExecuteReader();
            List<Tuple<string, string>> words = new List<Tuple<string, string>>();
            while (reader.Read())
            {
               
                String hwd =(String) reader["hwd"];
                String ID = (String)reader["id"];
                //Console.WriteLine(hwd);
                var normalizedString = hwd.Normalize(NormalizationForm.FormD);
                Regex regSpecialSymbols=new Regex("\\W");
                normalizedString=regSpecialSymbols.Replace(normalizedString,"");
                StringBuilder convertedString = new StringBuilder();
                foreach (var charactr in normalizedString)
                {
                    var currentUnicodeCat = CharUnicodeInfo.GetUnicodeCategory(charactr);
                    if (currentUnicodeCat != UnicodeCategory.NonSpacingMark )
                    {
                        convertedString.Append(charactr);
                    }
                }
                String convertedstring = convertedString.ToString().Normalize(NormalizationForm.FormC);
                Tuple<String, String> Item = new Tuple<String, String>(ID,convertedstring);
                words.Add(Item);

            }
            Console.WriteLine(words.Count);
            //Updating database with new column
            string altertable = @"Alter table lookup add COLUMN updatedhwd varchar";
            SQLiteCommand cmd2 = new SQLiteCommand(altertable, con);
            cmd2.ExecuteNonQuery();
            var transaction = con.BeginTransaction();
            int cnt = 1;
            foreach (Tuple<String, String> obj in words)
            {
                String ID = obj.Item1;
                String updatedword = obj.Item2;
                string updateQuery = String.Format("UPDATE LOOKUP SET updatedhwd='{0}' where id='{1}'",updatedword,ID);
                SQLiteCommand newcmd = new SQLiteCommand(updateQuery, con);
                newcmd.CommandType = CommandType.Text;
                try
                {
                    newcmd.ExecuteNonQuery();
                    Console.WriteLine(cnt++);
                 }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured");
                }
            }

            transaction.Commit();
            con.Close();
        }


        #endregion


        #region avoidSpecialsymbols

        #endregion

        static void Main(string[] args)
        {
            SQLiteConnection.CreateFile("LDOCE.sqlite");
            //indexXML();
            //wordsets_linksXML();
            //verbtables_linkXML();
            //etymology_linksXML();
            //wordsetsXML();
            picsXML();
            exasoundXML();
            ussoundXML();
            uksoundXML();
            verbtablesXML();
            etymologyXML();
            coreXML();
            createLookupFromCore();
            coreSearchInflections();
            Console.WriteLine("Total time taken: " + totalTime / 1000);
            Console.Read();

        }
    }
}
