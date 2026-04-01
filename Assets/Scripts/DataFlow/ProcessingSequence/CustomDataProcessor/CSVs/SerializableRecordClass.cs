using System.Collections.Generic;
using CsvHelper.Configuration;
using DracoRuan.Foundation.DataFlow.LocalData;

namespace DracoRuan.Foundation.DataFlow.ProcessingSequence.CustomDataProcessor.CSVs
{
    public class SerializableRecordClass<TRecord, TRecordMap> : ISetCustomCsvRecordGameData<TRecord>
        where TRecord : class
        where TRecordMap : ClassMap<TRecord>
    {
        public IEnumerable<TRecord> Records { get; private set; }

        public void SetCustomGameDataRecords(IEnumerable<TRecord> recordDataFromCsv)
        {
            this.Records = recordDataFromCsv;
        }
    }
}