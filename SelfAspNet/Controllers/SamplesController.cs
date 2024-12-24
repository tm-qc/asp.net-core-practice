using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SelfAspNet.Models;

namespace SelfAspNet.Controllers
{
    public class SamplesController : Controller
    {
        private readonly MyContext _context;

        public SamplesController(MyContext context)
        {
            _context = context;
        }

        // GET: Samples
        // - <IActionResult>：非同期(async await)の戻り値の型
        // - awaitの処理が終わって次の処理に行く。非同期と違い同期処理にする
        //   (非同期処理って命名がわかりづらいが、要は同期処理)
        // - ToListAsync()はコレクションでデータを非同期的に取得する

        // 非同期処理とは？
        // 「非同期」の本質は、「待ち時間にスレッドを解放して他の作業を進められること」 です。
        // 一見すると同期的に見える動作（結果を待つ）をしつつ、内部的にはリソースを効率的に使っています。
        
        // 非同期に効率よく処理しつつ、最終的に同期されて順番に処理をするということ


        public async Task<IActionResult> Index()
        {
            return View(await _context.Samples.ToListAsync());
        }

        // GET: Samples/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sample = await _context.Samples
                .FirstOrDefaultAsync(m => m.id == id);
            if (sample == null)
            {
                return NotFound();
            }

            return View(sample);
        }

        // GET: Samples/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Samples/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,title,sub_title")] Sample sample)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sample);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sample);
        }

        // GET: Samples/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sample = await _context.Samples.FindAsync(id);
            if (sample == null)
            {
                return NotFound();
            }
            return View(sample);
        }

        // POST: Samples/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,title,sub_title")] Sample sample)
        {
            if (id != sample.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sample);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SampleExists(sample.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sample);
        }

        // GET: Samples/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sample = await _context.Samples
                .FirstOrDefaultAsync(m => m.id == id);
            if (sample == null)
            {
                return NotFound();
            }

            return View(sample);
        }

        // POST: Samples/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sample = await _context.Samples.FindAsync(id);
            if (sample != null)
            {
                _context.Samples.Remove(sample);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SampleExists(int id)
        {
            return _context.Samples.Any(e => e.id == id);
        }
    }
}
