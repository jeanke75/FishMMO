﻿using System;
using System.Linq;
using FishMMO.Database.Npgsql;
using FishMMO.Database.Npgsql.Entities;

namespace FishMMO.Server.DatabaseServices
{
	public class SceneServerService
	{
		/// <summary>
		/// Adds a new server to the server list. The Login server will fetch this list for new clients.
		/// </summary>
		public static SceneServerEntity Add(
			NpgsqlDbContext dbContext,
			string address,
			ushort port,
			int characterCount,
			bool locked,
			out long id
		)
		{
			var server = new SceneServerEntity()
			{
				LastPulse = DateTime.UtcNow,
				Address = address,
				Port = port,
				CharacterCount = characterCount,
				Locked = locked
			};
			dbContext.SceneServers.Add(server);
			dbContext.SaveChanges();

			id = server.ID;
			return server;
		}

		public static void Pulse(NpgsqlDbContext dbContext, long id, int characterCount)
		{
			var sceneServer = dbContext.SceneServers.FirstOrDefault(c => c.ID == id);
			if (sceneServer == null) throw new Exception($"Couldn't find Scene Server with ID: {id}");

			sceneServer.LastPulse = DateTime.UtcNow;
			sceneServer.CharacterCount = characterCount;
			dbContext.SaveChanges();
		}

		public static void Delete(NpgsqlDbContext dbContext, long id)
		{
			var sceneServer = dbContext.SceneServers.FirstOrDefault(c => c.ID == id);
			if (sceneServer != null)
			{
				dbContext.SceneServers.Remove(sceneServer);
				dbContext.SaveChanges();
			}
		}

		public static SceneServerEntity GetServer(NpgsqlDbContext dbContext, long id)
		{
			var sceneServer = dbContext.SceneServers.FirstOrDefault(c => c.ID == id);
			if (sceneServer == null) throw new Exception($"Couldn't find Scene Server with ID: {id}");

			return sceneServer;
		}
	}
}