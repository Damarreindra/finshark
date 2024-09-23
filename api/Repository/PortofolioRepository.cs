using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class PortofolioRepository : IPortofolioRepository
    {   
        private readonly ApplicationDBContext _context;
        public PortofolioRepository(ApplicationDBContext context)
        {   
            _context = context;
        }

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
           await _context.Portfolios.AddAsync(portfolio);
           await _context.SaveChangesAsync();

           return portfolio;
        }

        public async Task<Portfolio> Delete(AppUser user, string symbol)
        {
            var portofolioModel = await _context.Portfolios.FirstOrDefaultAsync(x => x.AppUserId == user.Id && x.Stock.Symbol.ToLower() == symbol.ToLower());
            if(portofolioModel == null) return null;

            _context.Portfolios.Remove(portofolioModel);
            await _context.SaveChangesAsync();
            return portofolioModel;
        }

        public async Task<List<Stock>> GetUserPortofolio(AppUser user)
        {
            return await _context.Portfolios.Where(u=>u.AppUserId == user.Id)
            .Select(stock => new Stock{
                Id = stock.StockId,
                Symbol = stock.Stock.Symbol,
                CompanyName = stock.Stock.CompanyName,
                Purchase = stock.Stock.Purchase,
                LastDiv = stock.Stock.LastDiv,
                Industry = stock.Stock.Industry,
                MarketCap = stock.Stock.MarketCap
            }).ToListAsync();
        }
    }
}