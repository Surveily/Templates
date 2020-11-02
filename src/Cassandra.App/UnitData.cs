// <copyright file="UnitData.cs" company="VAR Unit">
// Copyright (c) VAR Unit. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Cassandra
{
    public class UnitData : TableEntity
    {
        private string _valuesJson;
        private Dictionary<string, string> _values;

        public Guid UnitId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PartitionKey))
                {
                    return Guid.Empty;
                }

                return Guid.Parse(PartitionKey);
            }
            set => PartitionKey = value.ToString();
        }

        public Guid Id
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RowKey))
                {
                    return Guid.Empty;
                }

                return Guid.Parse(RowKey.Split('+')[2]);
            }
            set => RowKey = $"{Occurred.ToRowKey()}+{ContractId}+{value}";
        }

        public Guid ContractId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RowKey))
                {
                    return Guid.Empty;
                }

                return Guid.Parse(RowKey.Split('+')[1]);
            }
            set => RowKey = $"{Occurred.ToRowKey()}+{value}+{Id}";
        }

        public DateTime Occurred
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RowKey))
                {
                    return default(DateTime);
                }

                return new DateTime(long.Parse(RowKey.Split('+')[0]), DateTimeKind.Utc);
            }
            set => RowKey = $"{value.ToRowKey()}+{ContractId}+{Id}";
        }

        public Guid AccountId { get; set; }

        public Guid LocationId { get; set; }

        [JsonIgnore]
        public string ValuesJson
        {
            get
            {
                return _valuesJson;
            }

            set
            {
                _valuesJson = value;

                try
                {
                    _values = value == null ? null
                                            : JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
                }
                catch (JsonReaderException)
                {
                    _values = null;
                }
            }
        }

        [IgnoreProperty]
        public Dictionary<string, string> Values
        {
            get => _values;
            set
            {
                _values = value;
                _valuesJson = JsonConvert.SerializeObject(value);
            }
        }

        public UnitDataById ById()
        {
            return new UnitDataById
            {
                Id = Id,
                UnitId = UnitId,
                Identifier = RowKey,
            };
        }
    }

    public class UnitDataById : TableEntity
    {
        public Guid UnitId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PartitionKey))
                {
                    return Guid.Empty;
                }

                return Guid.Parse(PartitionKey);
            }
            set => PartitionKey = value.ToString();
        }

        public Guid Id
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RowKey))
                {
                    return Guid.Empty;
                }

                return Guid.Parse(RowKey);
            }
            set => RowKey = value.ToString();
        }

        public string Identifier { get; set; }
    }
}