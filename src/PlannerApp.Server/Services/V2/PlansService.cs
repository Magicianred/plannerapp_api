﻿using PlannerApp.Models;
using PlannerApp.Models.V2.DTO;
using PlannerApp.Server.Data;
using PlannerApp.Server.Exceptions;
using PlannerApp.Server.Interfaces;
using PlannerApp.Server.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace PlannerApp.Server.Services.V2
{
    public class PlansService : Interfaces.IPlansService
    {

        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storage;
        private readonly IdentityOptions _identity;

        public PlansService(ApplicationDbContext db, IStorageService storage, IdentityOptions identity)
        {
            _db = db;
            _storage = storage;
            _identity = identity;
        }

        public async Task<PlanDetail> CreateAsync(PlanDetail model)
        {
            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string coverUrl = "https://akacademy.blob.core.windows.net/plannerapp/EwRGr1DXAAEXNK1.jpg"; // TODO: Update the default picture uploaded
                if (model.CoverFile != null)
                    coverUrl = await _storage.SaveBlobAsync(model.CoverFile, Models.BlobType.Image);

                var plan = new Plan
                {
                    Id = Guid.NewGuid().ToString(),
                    CoverPath = coverUrl,
                    CreatedDate = DateTime.UtcNow,
                    Description = model.Description?.Trim(),
                    Title = model.Title.Trim(),
                    ModifiedDate = DateTime.UtcNow,
                    UserId = _identity.UserId,
                };
                await _db.Plans.AddAsync(plan);
                await _db.SaveChangesAsync();

                ts.Complete(); 
                return null;
            }
        }

        public async Task DeleteAsync(string id)
        {
            var plan = await _db.Plans.FindAsync(id);
            if (plan == null)
                throw new NotFoundException($"Plan with the Id: {id} not found");

            plan.IsDeleted = true;
            plan.ModifiedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync(); 
        }

        public async Task<PlanDetail> EditAsync(PlanDetail model)
        {
            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var plan = await _db.Plans.FindAsync(model.Id);
                if (plan == null)
                    throw new NotFoundException($"Plan with the Id: {model.Id} not found");

                var url = plan.CoverPath;
                var old = plan.CoverPath;
                if (model.CoverFile != null)
                    url = await _storage.SaveBlobAsync(model.CoverFile, Models.BlobType.Image);

                plan.Title = model.Title;
                plan.Description = model.Description;
                plan.ModifiedDate = DateTime.UtcNow;
                plan.CoverPath = url;

                await _db.SaveChangesAsync();

                if (old.Contains("default.jpg"))
                    await _storage.RemoveAsync(old);

                return null;
            }
        }

        public Task<PlanDetail> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<PlanDetail>> GetPlans(string query, int page = 1, int pageSize = 12)
        {
            throw new NotImplementedException();
        }
    }
}
