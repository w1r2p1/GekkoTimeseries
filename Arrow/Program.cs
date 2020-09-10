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
        public static string _file = @"c:\Thomas\Desktop\gekko\testing\test.arrow";

        //[STAThread]
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

            Gekko.Globals.arrow = true;  //so that messages are not shown
            Gekko.Program.databanks.storage.Add(new Databank("Work"));
            O.Read o0 = new O.Read();            
            o0.type = @"read";
            o0.fileName = @"c:\Thomas\Desktop\gekko\testing\jul05";
            o0.opt_first = "yes";
            o0.Exe();
            Databank db = Gekko.Program.databanks.GetFirst();
            string s = Globals.unitTestScreenOutput.ToString();

            int t1 = 1970;
            int t2 = 2020;
            int n = t2 - t1 + 1;
            int k = db.storage.Count;

            DateTime dt1 = DateTime.Now;
            List<DataFrameColumn> list = new List<DataFrameColumn>(k);
            //List<DataFrameColumn> newColumns = new List<DataFrameColumn>(n);
            
            StringDataFrameColumn indexColumn = new StringDataFrameColumn("time", n);
            foreach (GekkoTime t in new GekkoTimeIterator(new GekkoTime(EFreq.A, 1970, 1), new GekkoTime(EFreq.A, 2020, 1)))
            {
                indexColumn.Add<string>(t.super.ToString());
            }
            //list.Add(indexColumn);
            foreach (KeyValuePair<string, IVariable> kvp in db.storage)
            {
                PrimitiveDataFrameColumn<double> column = new PrimitiveDataFrameColumn<double>(kvp.Key, n);
                foreach (GekkoTime t in new GekkoTimeIterator(new GekkoTime(EFreq.A, 1970, 1), new GekkoTime(EFreq.A, 2020, 1)))
                {
                    Series ts = kvp.Value as Series;
                    column.Add<double>(ts.GetDataSimple(t));
                }
                //df2.Add<PrimitiveDataFrameColumn<double>>(xx);
                //df2.Add<PrimitiveDataFrameColumn<double>>(list);
                //newColumns.Add(xx);
                list.Add(column);
            }                        
            DataFrame df = new DataFrame(list);
            MessageBox.Show("Construct arrow took: " + (DateTime.Now - dt1).TotalMilliseconds / 1000d);

            dt1 = DateTime.Now;
            var batches = df.ToArrowRecordBatches();
            WriteArrow(batches, _file);
            MessageBox.Show("Write arrow took: " + (DateTime.Now - dt1).TotalMilliseconds / 1000d);

            if (false)
            {
                Task t = Main2(args);
            }

            dt1 = DateTime.Now;
            Task<RecordBatch> tt = ReadArrowAsync(_file);
            RecordBatch rb = tt.Result;
            DataFrame df2 = DataFrame.FromArrowRecordBatch(rb);
            MessageBox.Show("Read arrow took: " + (DateTime.Now - dt1).TotalMilliseconds / 1000d);
            //IEnumerable<RecordBatch> rb2 = df2.ToArrowRecordBatches();
        }

        public static async Task<RecordBatch> ReadArrowAsync(string filename)
        {
            using (var stream = File.OpenRead(filename))
            using (var reader = new ArrowFileReader(stream))
            {
                var recordBatch = await reader.ReadNextRecordBatchAsync();
                //Debug.WriteLine("Read record batch with {0} column(s)", recordBatch.ColumnCount);
                return recordBatch;
            }
        }

        public static void WriteArrow(IEnumerable<RecordBatch> batches, string fileName)
        {
            using (var stream = File.OpenWrite(fileName))
            using (var writer = new ArrowFileWriter(stream, batches.First().Schema))
            {
                foreach (RecordBatch b in batches)
                {
                    //DataFrame ddf7 = DataFrame.FromArrowRecordBatch(b);
                    writer.WriteRecordBatchAsync(b).Wait();
                }
                writer.WriteEndAsync();
            }
        }

        public static async Task Main2(string[] args)
        {
            // Use a specific memory pool from which arrays will be allocated (optional)

            var memoryAllocator = new NativeMemoryAllocator(alignment: 64);

            // Build a record batch using the Fluent API

            RecordBatch recordBatch = new RecordBatch.Builder(memoryAllocator)
                .Append("Column A", false, col => col.Int32(array => array.AppendRange(Enumerable.Range(0, 10))))
                .Append("Column B", false, col => col.Float(array => array.AppendRange(Enumerable.Range(0, 10).Select(x => Convert.ToSingle(x * 2)))))
                .Append("Column C", false, col => col.String(array => array.AppendRange(Enumerable.Range(0, 10).Select(x => $"Item {x + 1}"))))
                .Append("Column D", false, col => col.Boolean(array => array.AppendRange(Enumerable.Range(0, 10).Select(x => x % 2 == 0))))
                .Build();

            File.Delete(_file);
            MemoryStream ms = new MemoryStream();
            ArrowStreamWriter writer = new ArrowStreamWriter(ms, recordBatch.Schema);
            await writer.WriteRecordBatchAsync(recordBatch);
            //Hmmm, it seems the "magic" ARROW1 is not present at the start of the file
            using (FileStream file = new FileStream(_file, FileMode.Create, System.IO.FileAccess.Write)) ms.WriteTo(file);

        }

    }
}