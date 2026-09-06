using alnakhil.Data;
using alnakhil.Models;
using alnakhil.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace alnakhil.Controllers
{
    public class DebtsController : Controller
    {
        private readonly alnakhilContext _context;

        public DebtsController(alnakhilContext context)
        {
            _context = context;
        }

        // ================== INDEX ==================
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Index()
        {
            var payables = await _context.Purchases
                .Where(p => p.PaymentStatus != PaymentStatus.Paid)
                .GroupBy(p => p.SupplierName)
                .Select(g => new DebtSummaryVM
                {
                    Name = g.Key,
                    Type = DebtType.Payable,
                    TotalRemaining = g.Sum(x => x.TotalAmount - x.AmountPaid),
                    InvoicesCount = g.Count(),
                    NearestDueDate = g.Min(x => x.DueDate)
                })
                .ToListAsync();

            var receivables = await _context.Sales
                .Where(s => s.PaymentStatus != PaymentStatus.Paid)
                .GroupBy(s => s.CustomerName ?? "عميل نقدي")
                .Select(g => new DebtSummaryVM
                {
                    Name = g.Key,
                    Type = DebtType.Receivable,
                    TotalRemaining = g.Sum(x => x.TotalAmount - x.AmountPaid),
                    InvoicesCount = g.Count(),
                    NearestDueDate = g.Min(x => x.DueDate)
                })
                .ToListAsync();

            var model = payables.Concat(receivables).ToList();

            return View(model);
        }

        // ================== DETAILS ==================
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Details(string name, DebtType type)
        {
            var model = new DebtDetailsVM
            {
                Name = name,
                Type = type
            };

            if (type == DebtType.Payable)
            {
                var purchases = await _context.Purchases
                    .Where(p => p.SupplierName == name)
                    .ToListAsync();

                model.Invoices = purchases.Select(p => new DebtVM
                {
                    Id = p.Id,
                    InvoiceNumber = p.InvoiceNumber,
                    InvoiceDate = p.PurchaseDate,
                    DueDate = p.DueDate,
                    TotalAmount = p.TotalAmount,
                    AmountPaid = p.AmountPaid,
                    RemainingAmount = p.TotalAmount - p.AmountPaid,
                    PaymentStatus = p.PaymentStatus,
                    Type = DebtType.Payable
                }).ToList();

                model.TotalAmount = purchases.Sum(x => x.TotalAmount);
                model.TotalPaid = purchases.Sum(x => x.AmountPaid);
            }
            else
            {
                var sales = await _context.Sales
                    .Where(s => (s.CustomerName ?? "عميل نقدي") == name)
                    .ToListAsync();

                model.Invoices = sales.Select(s => new DebtVM
                {
                    Id = s.Id,
                    InvoiceNumber = s.InvoiceNumber,
                    InvoiceDate = s.SaleDate,
                    DueDate = s.DueDate,
                    TotalAmount = s.TotalAmount,
                    AmountPaid = s.AmountPaid,
                    RemainingAmount = s.TotalAmount - s.AmountPaid,
                    PaymentStatus = s.PaymentStatus,
                    Type = DebtType.Receivable
                }).ToList();

                model.TotalAmount = sales.Sum(x => x.TotalAmount);
                model.TotalPaid = sales.Sum(x => x.AmountPaid);
            }

            model.TotalRemaining = model.TotalAmount - model.TotalPaid;

            return View(model);
        }

        // ================== PAY ==================
        [Authorize(Roles = "Admin,Cashier")]
        [HttpPost]
        public async Task<IActionResult> Pay(int id, decimal amount, DebtType type, string? notes = null)
        {
            if (amount <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            if (type == DebtType.Payable)
            {
                var purchase = await _context.Purchases.FindAsync(id);
                if (purchase == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                purchase.AmountPaid += amount;

                if (purchase.AmountPaid >= purchase.TotalAmount)
                    purchase.PaymentStatus = PaymentStatus.Paid;
                else
                    purchase.PaymentStatus = PaymentStatus.Deferred;

                _context.DebtPayments.Add(new DebtPayment
                {
                    PurchaseId = purchase.Id,
                    Amount = amount,
                    PaymentDate = DateTime.Now,
                    Notes = notes
                });

                await _context.SaveChangesAsync();
            }
            else
            {
                var sale = await _context.Sales.FindAsync(id);
                if (sale == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                sale.AmountPaid += amount;

                if (sale.AmountPaid >= sale.TotalAmount)
                    sale.PaymentStatus = PaymentStatus.Paid;
                else
                    sale.PaymentStatus = PaymentStatus.Deferred;

                _context.DebtPayments.Add(new DebtPayment
                {
                    SaleId = sale.Id,
                    Amount = amount,
                    PaymentDate = DateTime.Now,
                    Notes = notes
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}