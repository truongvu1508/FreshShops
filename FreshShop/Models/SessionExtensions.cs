using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace FreshShop.Models
{
	public static class SessionExtensions
	{
		// Serialize the object to JSON and store it in session
		public static void SetObjectAsJson(this ISession session, string key, object value)
		{
			var json = JsonConvert.SerializeObject(value);
			session.SetString(key, json);
		}

		// Retrieve the object from session by deserializing the JSON string
		public static T GetObjectFromJson<T>(this ISession session, string key)
		{
			var json = session.GetString(key);
			return json == null ? default : JsonConvert.DeserializeObject<T>(json);
		}
	}
}
