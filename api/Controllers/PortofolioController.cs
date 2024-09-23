using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portofolio")]
    [ApiController]
    public class PortofolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPortofolioRepository _portofolioRepository;
        public PortofolioController(UserManager<AppUser> userManager, IStockRepository stockRepository, IPortofolioRepository portofolioRepository)
        {
            _userManager = userManager;
            _stockRepo = stockRepository;
            _portofolioRepository = portofolioRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortofolio(){
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var userPortofolio = await _portofolioRepository.GetUserPortofolio(appUser);
            return Ok(userPortofolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePortofolio(string symbol){
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var stock = await _stockRepo.GetBySymbol(symbol);

            if(stock == null) return BadRequest("Stock Not Found");

            var userPortofolio = await _portofolioRepository.GetUserPortofolio(appUser);
            if(userPortofolio.Any(e => e.Symbol.ToLower() == symbol.ToLower())) return BadRequest(new{message = "Cannot add same Stock!"});

            var portofolioModel = new Portfolio{
                StockId = stock.Id,
                AppUserId = appUser.Id
            };

            await _portofolioRepository.CreateAsync(portofolioModel);

            if(portofolioModel == null){
                return StatusCode(500, "Create Error");
            }else{
                return Created();
            };

        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortofolio(string symbol){
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var userPortofolio = await _portofolioRepository.GetUserPortofolio(appUser);
            var filterdStock = userPortofolio.Where(s=>s.Symbol.ToLower() == symbol.ToLower()).ToList();

            if(filterdStock.Count()== 1){
                await _portofolioRepository.Delete(appUser, symbol);
            }else{
                return BadRequest("Stock not in your portofolio");
            };

            return Ok();

        }
    }
}