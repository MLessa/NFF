﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFreeV2.Models.Integration
{

	public class BaseResponse<T>
	{
		public T Data { get; set; }
		public string Message { get; set; }
		public bool Success { get; set; }
	}
}