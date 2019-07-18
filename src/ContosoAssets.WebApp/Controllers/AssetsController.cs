using System;
using System.Linq;
using System.Threading.Tasks;
using ContosoAssets.SolutionManagement.SubscriptionManagement;
using ContosoAssets.Utils;
using ContosoAssets.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoAssets.WebApp.Controllers
{
    // See https://github.com/aspnet/AspNetCore/blob/master/src/Identity/Core/src/IdentityConstants.cs#L15
    [Authorize(Roles = Constants.CustomerUserRoleName, AuthenticationSchemes = "Identity.Application")]
    public class AssetsController : MeteredController
    {
        private const string upsellMessage =
            "Usage is over selected product plan limits. Please contact your administrator to upgrade.";

        private readonly AssetManagementDbContext _context;

        public AssetsController(AssetManagementDbContext context, ISubscriptionManager skuManager) : base(skuManager)
        {
            this._context = context;
        }

        // GET: Assets/Create
        public IActionResult Create()
        {
            return this.View();
        }

        // POST: Assets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,LastUpdateTime,Status")]
            Asset asset)
        {
            if (!await this.CheckLimitAndMeter())
            {
                this.ModelState.AddModelError("Usage", upsellMessage);
                return this.View("AddAssetError");
            }

            if (this.ModelState.IsValid)
            {
                asset.LastUpdateTime = DateTimeOffset.UtcNow;
                asset.CustomerName = this.User.Identity.Name.GetDomainNameFromEmail();
                asset.Id = Guid.NewGuid().ToString();
                this._context.Add(asset);
                await this._context.SaveChangesAsync();
                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(asset);
        }

        // GET: Assets/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var asset = await this._context.Assets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asset == null)
            {
                return this.NotFound();
            }

            return this.View(asset);
        }

        // POST: Assets/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await this.CheckLimitAndMeter();

            var asset = await this._context.Assets.FindAsync(id);
            this._context.Assets.Remove(asset);
            await this._context.SaveChangesAsync();
            return this.RedirectToAction(nameof(this.Index));
        }

        // GET: Assets/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var asset = await this._context.Assets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asset == null)
            {
                return this.NotFound();
            }

            return this.View(asset);
        }

        // GET: Assets/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var asset = await this._context.Assets.FindAsync(id);
            if (asset == null)
            {
                return this.NotFound();
            }

            return this.View(asset);
        }

        // POST: Assets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,LastUpdateTime,Status")]
            Asset asset)
        {
            if (!await this.CheckLimitAndMeter())
            {
                this.ModelState.AddModelError("Usage", upsellMessage);
                return this.View(asset);
            }

            if (id != asset.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                try
                {
                    var existingAsset = await this._context.Assets.FirstOrDefaultAsync(a => a.Id == asset.Id);

                    if (existingAsset != default(Asset))
                    {
                        existingAsset.LastUpdateTime = DateTimeOffset.UtcNow;
                        existingAsset.Status = asset.Status;

                        this._context.Update(existingAsset);
                        await this._context.SaveChangesAsync();
                    }
                    else
                    {
                        return this.NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.AssetExists(asset.Id))
                    {
                        return this.NotFound();
                    }

                    throw;
                }

                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(asset);
        }

        // GET: Assets
        public async Task<IActionResult> Index()
        {
            return this.View(await this._context.Assets
                .Where(a => a.CustomerName == this.User.Identity.Name.GetDomainNameFromEmail()).ToListAsync());
        }

        private bool AssetExists(string id)
        {
            return this._context.Assets.Any(e => e.Id == id);
        }
    }
}
