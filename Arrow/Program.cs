﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.Arrow;
using Apache.Arrow.Ipc;
using Apache.Arrow.Memory;
using System.IO;
using Microsoft.Data.Analysis;

using Gekko;
using System.Windows.Forms;

namespace Arrow
{
    public class Program
    {
        public static string _fileName = @"c:\Thomas\Desktop\gekko\testing\test.arrow";
                
        public static void Main(string[] args)
        {
            //DataFrame project in .NET
            //See this thread: https://github.com/dotnet/runtime/issues/24920
            //Eric Erhardt from MS has been involved in arrow/C#
            //https://devblogs.microsoft.com/dotnet/an-introduction-to-dataframe/    
            //See this: https://stackoverflow.com/questions/56231247/numpy-pandas-counterpart-in-net-or-netcore/56280314#56280314
            //or better this: https://www.nuget.org/packages/Microsoft.Data.Analysis/
            //github: https://github.com/dotnet/corefxlab/tree/master/src/Microsoft.Data.Analysis            
            //regarding python, see also Eviews: http://www.eviews.com/download/whitepapers/pyeviews.pdf
            //regarding R, see also EViews: https://www.eviews.com/download/whitepapers/Using%20R%20with%20EViews.pdf
            //If RAM is supposed to be shared, arrow uses Googles gRPC library
            //Compression with LZ4 should probably be the standard, but how to do this from C#? See https://ursalabs.org/blog/2020-feather-v2/

            // --------> example
            //PrimitiveDataFrameColumn<DateTime> dateTimes = new PrimitiveDataFrameColumn<DateTime>("DateTimes"); // Default length is 0.
            //PrimitiveDataFrameColumn<int> ints = new PrimitiveDataFrameColumn<int>("Ints", 3); // Makes a column of length 3. Filled with nulls initially
            //StringDataFrameColumn strings = new StringDataFrameColumn("Strings", 3); // Makes a column of length 3. Filled with nulls initially
            ////Append 3 values to dateTimes
            //dateTimes.Append(DateTime.Parse("2019/01/01"));
            //dateTimes.Append(DateTime.Parse("2019/01/01"));
            //dateTimes.Append(DateTime.Parse("2019/01/02"));
            //DataFrame df = new DataFrame(dateTimes, ints, strings); // This will throw if the columns are of different lengths            
            //df[0, 1] = 10; // 0 is the rowIndex, and 1 is the columnIndex. This sets the 0th value in the Ints columns to 10
            //// Modify ints and strings columns by indexing
            //ints[1] = 100;
            //strings[1] = "Foo!";
            //df.Info();
            //// Add 5 to Ints through the DataFrame
            //df.Columns["Ints"].Add(5, inPlace: true);
            //DataFrameRow row0 = df.Rows[0];
            //for (long i = 0; i < df.Rows.Count; i++)
            //{
            //    DataFrameRow row = df.Rows[i];
            //}
            //// Filter rows based on equality
            //PrimitiveDataFrameColumn<bool> boolFilter = df.Columns["Strings"].ElementwiseEquals("Bar");
            //DataFrame filtered = df.Filter(boolFilter);       

            string s, s0, s1, s2, s3;

            DateTime dt1 = DateTime.Now;

            if (true)
            {
                dt1 = DateTime.Now;
                Globals.unitTestScreenOutput.Clear();
                Gekko.Globals.arrow = true;  //so that messages are not shown
                Gekko.Program.databanks.storage.Add(new Databank("Work"));
                O.Read o0 = new O.Read();
                o0.type = @"read";
                o0.fileName = @"c:\Thomas\Desktop\gekko\testing\jul05";
                o0.opt_first = "yes";
                o0.Exe();
                Databank db = Gekko.Program.databanks.GetFirst();
                s = Globals.unitTestScreenOutput.ToString();
                s0 = "Read gbk took: " + (DateTime.Now - dt1).TotalMilliseconds / 1000d;


                dt1 = DateTime.Now;
                int t1 = 1998;
                int t2 = 2079;
                int n = t2 - t1 + 1;
                int k = db.storage.Count;
                
                List<DataFrameColumn> list = new List<DataFrameColumn>(k);                

                StringDataFrameColumn indexColumn = new StringDataFrameColumn("time", n);
                foreach (GekkoTime t in new GekkoTimeIterator(new GekkoTime(EFreq.A, t1, 1), new GekkoTime(EFreq.A, t2, 1)))
                {
                    indexColumn.Add<string>(t.super.ToString());
                }
                //list.Add(indexColumn);
                
                int counter = 0;
                foreach (KeyValuePair<string, IVariable> kvp in db.storage)
                {
                    counter++;
                    PrimitiveDataFrameColumn<double> column = new PrimitiveDataFrameColumn<double>(kvp.Key, n);
                    int i = -1;
                    foreach (GekkoTime t in new GekkoTimeIterator(new GekkoTime(EFreq.A, t1, 1), new GekkoTime(EFreq.A, t2, 1)))
                    {
                        i++;
                        Series ts = kvp.Value as Series;
                        //column.Add<double>(ts.GetDataSimple(t));
                        column[i] = ts.GetDataSimple(t);
                    }
                    //df2.Add<PrimitiveDataFrameColumn<double>>(xx);
                    //df2.Add<PrimitiveDataFrameColumn<double>>(list);
                    //newColumns.Add(xx);
                    list.Add(column);
                    //if (counter > 10) break;
                }
                DataFrame df777 = new DataFrame(list);
                s1 = "Construct arrow took: " + (DateTime.Now - dt1).TotalMilliseconds / 1000d;

                //DataFrame df = new DataFrame(new PrimitiveDataFrameColumn<int>("Foo", 10), new PrimitiveDataFrameColumn<int>("Bar", Enumerable.Range(1, 10)));
                //RecordBatch recordBatch = new RecordBatch.Builder(new NativeMemoryAllocator(alignment: 64))
                //.Append("Column A", false, col => col.Int32(array => array.AppendRange(Enumerable.Range(0, 10))))
                //.Append("Column B", false, col => col.Float(array => array.AppendRange(Enumerable.Range(0, 10).Select(x => Convert.ToSingle(x * 2)))))
                //.Append("Column C", false, col => col.String(array => array.AppendRange(Enumerable.Range(0, 10).Select(x => $"Item {x + 1}"))))
                //.Append("Column D", false, col => col.Boolean(array => array.AppendRange(Enumerable.Range(0, 10).Select(x => x % 2 == 0))))
                //.Build();
                                               


                dt1 = DateTime.Now;
                //WriteArrow(recordBatch);
                WriteArrow(df777.ToArrowRecordBatches().First());
                //bool temp = task.Result; //actually runs it

                //dt1 = DateTime.Now;
                //var batches = df.ToArrowRecordBatches();
                //WriteArrow(batches, _file);


                s2 = "Write arrow took: " + (DateTime.Now - dt1).TotalMilliseconds / 1000d;

            }

            if (true)
            {
                dt1 = DateTime.Now;
                RecordBatch rb = ReadArrow(_fileName);
                DataFrame df2 = DataFrame.FromArrowRecordBatch(rb);
                s3 = "Read arrow took: " + (DateTime.Now - dt1).TotalMilliseconds / 1000d;
                //IEnumerable<RecordBatch> rb2 = df2.ToArrowRecordBatches();
            }

            Console.WriteLine(s);
            Console.WriteLine(s0);
            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);

            int ii = 1;
        }

        public static RecordBatch ReadArrow(string filename)
        {
            using (var stream = File.OpenRead(filename))
            using (var reader = new ArrowFileReader(stream))
            {
                //--->hmm cannot get async version to work, but recordBatches are for splitting very large datasets into batches of rows, 
                //so maybe not that important? But how are we sure all batches are read, if we only call "readNext"?
                //var recordBatch = await reader.ReadNextRecordBatchAsync(); 
                //See under usage here: https://github.com/apache/arrow/tree/master/csharp
                //In the following the non-async version:
                var recordBatch = reader.ReadNextRecordBatch();
                string s = "Read record batch with " + recordBatch.ColumnCount + " {0} column(s)";
                return recordBatch;
            }
        }

        public static async void WriteArrow(RecordBatch recordBatch)
        {
            // Use a specific memory pool from which arrays will be allocated (optional)

            File.Delete(_fileName);
                        
            MemoryStream stream = new MemoryStream();
            ArrowFileWriter writer = new ArrowFileWriter(stream, recordBatch.Schema, leaveOpen: true);
            await writer.WriteRecordBatchAsync(recordBatch);
            await writer.WriteEndAsync();
            using (FileStream fileStream = new FileStream(_fileName, FileMode.Create, System.IO.FileAccess.Write)) stream.WriteTo(fileStream);
            

        }

        public static void WriteArrow(IEnumerable<RecordBatch> batches, string fileName)
        {
            //DataFrame ddf7 = DataFrame.FromArrowRecordBatch(b);
            File.Delete(fileName);
            //using (var stream = File.OpenWrite(fileName))
            
            using (var stream = File.OpenWrite(fileName))
            using (var writer = new ArrowStreamWriter(stream, batches.First().Schema))
            {
                foreach (RecordBatch b in batches)
                {
                    writer.WriteRecordBatchAsync(b);
                    //writer.WriteRecordBatchAsync(b);
                }
                writer.WriteEndAsync();
                //writer.WriteEndAsync();

            }

        }
        

    }
}
