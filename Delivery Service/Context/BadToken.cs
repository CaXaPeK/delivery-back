using System;
using System.Collections.Generic;

namespace Delivery_Service.Context
{
    public partial class BadToken
    {
        public int Id { get; set; }

        public string? Value {  get; set; }

        public DateTime AddedAt {  get; set; }
    }
}
