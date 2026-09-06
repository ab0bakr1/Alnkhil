# PROJECT_CONTEXT.md — النخيل Smart POS
> توثيق شامل للمشروع — مُعدّ للقراءة من قِبل أي نموذج ذكاء اصطناعي أو مطور جديد.

---

## 1. نظرة عامة

### ما هو المشروع؟
**النخيل Smart POS** هو نظام نقطة بيع (Point of Sale) لإدارة سوبر ماركت. يغطّي دورة العمل الكاملة من الشراء من الموردين → إدارة المخزون → البيع للعملاء → تتبع الديون → التقارير والإحصاءات.

### من هم المستخدمون؟
- **Admin (مدير):** صلاحيات كاملة — إدارة المنتجات والمستخدمين والتقارير والإعدادات والمشتريات.
- **Cashier (كاشير):** صلاحيات محدودة — إنشاء فواتير البيع فقط.

### نوع المشروع
نظام ERP/POS داخلي (Internal Business Tool) — ليس SaaS عام، لكل متجر نسخته الخاصة مع قاعدة بيانات خاصة.

### حالة المشروع
**في التطوير الفعلي / يُستخدم محلياً** — المشروع يعمل ويُستخدم، والميزات الأساسية مكتملة. لا يوجد CI/CD أو نشر سحابي حتى الآن. آخر migration في فبراير 2026.

---

## 2. Tech Stack

### اللغات والـ Frameworks
| الطبقة | التقنية |
|--------|---------|
| **Backend** | ASP.NET Core 8.0 (MVC Pattern) |
| **Frontend** | Razor Views (.cshtml) + Vanilla JS + Bootstrap 5 RTL |
| **Database** | SQL Server (via Entity Framework Core 8.0) |
| **Auth** | ASP.NET Core Identity |
| **Language** | C# (.NET 8) |

### المكتبات الأساسية
| المكتبة | الإصدار | السبب |
|---------|---------|-------|
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 8.0.0 | نظام المصادقة والأدوار |
| `Microsoft.AspNetCore.Identity.UI` | 8.0.0 | صفحات Login/Logout الجاهزة |
| `Microsoft.EntityFrameworkCore` | 8.0.0 | ORM للتعامل مع قاعدة البيانات |
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.0 | Provider لـ SQL Server |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.0 | أدوات الـ Migrations |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | 8.0.0 | Scaffolding |

**مكتبات Frontend (من wwwroot/lib):**
- Bootstrap 5 RTL (النسخة العربية)
- jQuery
- Bootstrap Icons

**من CDN:**
- Google Fonts — خط Cairo
- Bootstrap Icons CDN

### أدوات البناء والنشر
- **Build:** `dotnet build` / Visual Studio
- **Database:** EF Core Migrations (`dotnet ef migrations add` / `dotnet ef database update`)
- **Hosting:** محلي فقط حالياً (IIS Express / Kestrel)
- **لا يوجد CI/CD**

### إصدارات مهمة
- **.NET:** 8.0
- **Target Framework:** `net8.0`
- **Nullable:** مُفعّل
- **Implicit Usings:** مُفعّل
- **Package Manager:** NuGet

---

## 3. هيكل المجلدات

```
Alnkhil/
├── Areas/
│   └── Identity/
│       └── Pages/
│           └── Account/          ← صفحات Login/Logout/Register (من Identity UI)
├── Controllers/                  ← جميع controllers الخاصة بالتطبيق
│   ├── CategoriesController.cs   ← إدارة التصنيفات (Admin فقط)
│   ├── DashboardController.cs    ← التقارير والإحصاءات (Admin فقط)
│   ├── DebtsController.cs        ← إدارة الديون (عام للمسجّلين)
│   ├── HomeController.cs         ← الصفحة الرئيسية
│   ├── InventoryControllers.cs   ← سجل حركة المخزون (Admin فقط)
│   ├── PricingController.cs      ← عرض وتعديل أسعار البيع (Admin فقط)
│   ├── PricingSettingsController.cs ← إعدادات الربح والضريبة (Admin فقط)
│   ├── ProductsController.cs     ← إدارة المنتجات — Edit/Delete فقط (Admin)
│   ├── PurchasesController.cs    ← فواتير الشراء (عام)
│   ├── SalesController.cs        ← فواتير البيع (Admin + Cashier)
│   └── UsersController.cs        ← إدارة المستخدمين/الكاشيرين (Admin فقط)
├── Data/
│   ├── alnakhilContext.cs        ← DbContext الرئيسي
│   └── DbInitializer.cs          ← Seed البيانات الأولية (Admin + الأدوار)
├── Helpers/
│   └── PricingHelper.cs          ← حساب سعر البيع تلقائياً
├── Migrations/                   ← جميع ملفات EF Core Migrations (61 ملف)
├── Models/                       ← نماذج قاعدة البيانات
│   ├── ApplicationUser.cs        ← امتداد IdentityUser (يُضيف FullName)
│   ├── Category.cs
│   ├── DebtPayment.cs
│   ├── ErrorViewModel.cs
│   ├── InventoryTransaction.cs
│   ├── PaymentStatus.cs          ← Enum: Unpaid/Paid/Deferred/Suspended
│   ├── PricingSetting.cs
│   ├── Product.cs
│   ├── Purchase.cs
│   ├── PurchaseItem.cs
│   ├── Sale.cs
│   ├── SaleItem.cs
│   ├── Supplier.cs
│   └── User.cs
├── Properties/
│   └── launchSettings.json       ← إعدادات التشغيل المحلي
├── Services/
│   ├── EmailSender.cs            ← Stub فارغ لـ IEmailSender (مطلوب من Identity)
│   └── InventoryService.cs       ← منطق تحديث المخزون عند الشراء/البيع/الإلغاء
├── ViewModels/                   ← نماذج العرض (DTO بين Controller و View)
│   ├── CreateCashierVM.cs
│   ├── DashboardVM.cs
│   ├── DebtDetailsVM.cs
│   ├── DebtSummaryVM.cs
│   ├── DebtVM.cs
│   ├── MonthlyStatsVM.cs
│   ├── PricingVM.cs
│   ├── ProductVM.cs
│   ├── PurchaseItemVM.cs
│   ├── PurchaseVM.cs
│   ├── SaleItemVM.cs
│   ├── SaleVM.cs
│   ├── TodayStatsVM.cs
│   └── UserWithRoleVM.cs
├── Views/                        ← صفحات Razor لكل Controller
│   ├── Categories/               ← Index, Create
│   ├── Dashboard/                ← Index, Today, Monthly
│   ├── Debts/                    ← Index, Details
│   ├── Home/                     ← Index (لوحة تحكم رئيسية), System, Privacy
│   ├── Inventory/                ← Index (سجل الحركة)
│   ├── Pricing/                  ← Index
│   ├── Products/                 ← Index, Edit, Delete
│   ├── Purchases/                ← Index, Create, Edit, Details
│   ├── Sales/                    ← Index, Create, Edit, Details
│   ├── Shared/
│   │   ├── _Layout.cshtml        ← القالب الرئيسي (Sidebar + TopBar)
│   │   ├── _LoginPartial.cshtml
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── Users/                    ← Index, Create, Edit, Delete, Details
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/
│   ├── css/                      ← ملفات CSS مخصصة
│   ├── js/                       ← ملفات JavaScript مخصصة
│   └── lib/                      ← Bootstrap, jQuery (من NuGet/libman)
├── alnakhil.csproj               ← ملف المشروع
├── alnakhil.sln                  ← ملف الـ Solution
├── appsettings.json              ← إعدادات الاتصال بقاعدة البيانات
├── appsettings.Development.json  ← إعدادات بيئة التطوير
├── Program.cs                    ← نقطة الدخول الرئيسية
└── TODO.md                       ← مهام سابقة (مكتملة كلها)
```

### الملفات المهمة
| الغرض | الملف |
|-------|-------|
| نقطة الدخول | `Program.cs` |
| DbContext | `Data/alnakhilContext.cs` |
| القالب العام | `Views/Shared/_Layout.cshtml` |
| حساب الأسعار | `Helpers/PricingHelper.cs` |
| منطق المخزون | `Services/InventoryService.cs` |
| إعداد قاعدة البيانات | `appsettings.json` |

---

## 4. المعمارية (Architecture)

### نمط التصميم
**MVC (Model-View-Controller)** — ASP.NET Core MVC الكلاسيكي. لا يوجد API منفصل؛ Frontend و Backend في نفس المشروع (Server-Side Rendering).

### كيف يتواصل Frontend مع Backend
1. **الطلبات العادية:** HTTP GET/POST عبر Razor Views + Form Submissions
2. **البحث الديناميكي (AJAX):** `fetch()` أو `jQuery.ajax()` إلى endpoints تُرجع JSON:
   - `GET /Sales/SearchProducts?query=...` — البحث عن منتج عند إنشاء فاتورة بيع
   - `GET /Purchases/Search?q=...` — البحث عن منتج عند إنشاء فاتورة شراء
   - `POST /Sales/Suspend` — حفظ فاتورة معلّقة (يُرجع JSON)
3. **لا يوجد SPA أو REST API منفصل** — كل شيء Server-Side.
4. **JsonOptions:** تم ضبط `PropertyNamingPolicy = null` لضمان إرجاع أسماء الخصائص كما هي (PascalCase).

### هيكل قاعدة البيانات

#### الجداول الرئيسية

```
AspNetUsers (Identity)
  └── ApplicationUser: IdentityUser
        + FullName (string?)

Category
  ├── Id (PK)
  └── Name (Required)

Product
  ├── Id (PK)
  ├── Name (Required, max 100)
  ├── Barcode (max 50)
  ├── Quantity (int)
  ├── PurchasePrice (decimal 18,2)   ← إجمالي سعر الشراء (مش سعر الوحدة)
  ├── SalePrice (decimal 18,2)       ← يُحسب تلقائياً أو يدوياً
  ├── IsManualPrice (bool)
  ├── ManualSalePrice (decimal?)
  ├── ExpirationDate (DateTime?)
  └── CategoryId (FK → Category, nullable)

Supplier
  ├── Id (PK)
  ├── Name (Required)
  └── Phone (string?)

Purchase
  ├── Id (PK)
  ├── PurchaseDate (DateTime)
  ├── InvoiceNumber (string?, max 50) ← يُولَّد بعد الحفظ: "PUR-{Id:00000}"
  ├── SupplierName (string, max 150)
  ├── SupplierId (FK → Supplier, nullable)
  ├── Subtotal / TaxPercentage / TaxAmount / DiscountPercentage / DiscountAmount / TotalAmount
  ├── AmountPaid (decimal)
  ├── PaymentStatus (enum)
  └── DueDate (DateTime?)

PurchaseItem
  ├── Id (PK)
  ├── PurchaseId (FK → Purchase)
  ├── ProductId (FK → Product)
  ├── CategoryId (FK → Category, nullable)
  ├── Quantity (int)
  ├── PurchasePrice (decimal) ← إجمالي سعر هذا الصنف (وليس سعر الوحدة)
  ├── ExpiryDate (DateTime?)
  └── [NotMapped] UnitPurchasePrice = PurchasePrice / Quantity

Sale
  ├── Id (PK)
  ├── SaleDate (DateTime)
  ├── InvoiceNumber (Required) ← "S{N:D6}" عادي أو "H{N:D6}" معلّق
  ├── CustomerName (string?)
  ├── Subtotal / TaxAmount / DiscountAmount / TotalAmount
  ├── AmountPaid (decimal)
  ├── IsSuspended (bool)
  ├── PaymentStatus (enum)
  └── DueDate (DateTime?)

SaleItem
  ├── Id (PK)
  ├── SaleId (FK → Sale)
  ├── ProductId (FK → Product)
  ├── Quantity (int)
  └── UnitPrice (decimal) ← سعر البيع وقت البيع

InventoryTransaction
  ├── Id (PK)
  ├── ProductId (FK → Product)
  ├── QuantityBefore / QuantityChanged / QuantityAfter
  ├── Type (enum: Purchase=1, Sale=2, Adjustment=3)
  ├── Date (DateTime)
  ├── UserName (string, max 100)
  └── Note (string?, max 200)

DebtPayment
  ├── Id (PK)
  ├── Amount (decimal)
  ├── PaymentDate (DateTime)
  ├── Notes (string?)
  ├── PurchaseId (FK → Purchase, nullable)
  └── SaleId (FK → Sale, nullable)

PricingSetting
  ├── Id (PK)
  ├── ProfitPercentage (decimal) ← نسبة الربح الافتراضية (مثال: 20)
  └── TaxPercentage (decimal)   ← نسبة الضريبة (مثال: 15)
```

#### العلاقات
- `Category` ← (1:N) → `Product`
- `Supplier` ← (1:N) → `Purchase`
- `Purchase` ← (1:N) → `PurchaseItem`
- `PurchaseItem` → `Product` (N:1)
- `Sale` ← (1:N) → `SaleItem`
- `SaleItem` → `Product` (N:1)
- `InventoryTransaction` → `Product` (N:1)
- `DebtPayment` → `Purchase` أو `Sale` (اختياري)
- `PricingSetting` جدول وحيد (صف واحد فقط في الغالب)

### نظام المصادقة (Auth)

**المزود:** ASP.NET Core Identity  
**نموذج المستخدم:** `ApplicationUser : IdentityUser` + حقل `FullName`

**الأدوار:**
| الدور | الصلاحيات |
|------|-----------|
| `Admin` | كل شيء |
| `Cashier` | إنشاء فواتير البيع فقط |

**تهيئة الأدوار:**  
- تُنشأ الأدوار (`Admin`, `Cashier`) تلقائياً عند تشغيل التطبيق في `Program.cs` (lines 43-56).
- `DbInitializer.SeedRolesAndAdminAsync()` موجود لكنه **غير مستدعى من Program.cs** (تعارض مع الكود في Program.cs — ينشئ `admin@admin.com` بكلمة `Admin@123` كـ fallback فقط).

**صفحات Identity:**  
- تُستخدم الصفحات الجاهزة من `Microsoft.AspNetCore.Identity.UI` عبر `Areas/Identity/Pages/Account/`.
- تسجيل الخروج: عبر POST form في `_LoginPartial.cshtml` أو `_Layout.cshtml`.

**إعدادات كلمة المرور** (مُخففة):
```
RequireDigit = false
RequiredLength = 6
RequireUppercase = false
RequireNonAlphanumeric = false
```

### الحضارة (Culture)
تم تثبيت الثقافة على `en-US` في نهاية `Program.cs` لضمان عمل أرقام الـ Decimal بشكل صحيح في جميع الطلبات.

---

## 5. الـ Routes والـ Endpoints

> **ملاحظة:** المشروع يستخدم MVC Routing التقليدي — لا يوجد REST API منفصل.  
> النمط الافتراضي: `{controller=Home}/{action=Index}/{id?}`

### SalesController — `/Sales`
| Method | Path | الوصف | الصلاحيات |
|--------|------|--------|-----------|
| GET | `/Sales` | قائمة جميع الفواتير | Admin, Cashier |
| GET | `/Sales/Details/{id}` | تفاصيل فاتورة | Admin, Cashier |
| GET | `/Sales/Create` | نموذج فاتورة جديدة | Admin, Cashier |
| POST | `/Sales/Create` | حفظ فاتورة جديدة | Admin, Cashier |
| POST | `/Sales/Suspend` | حفظ فاتورة معلّقة (JSON body) | Admin, Cashier |
| GET | `/Sales/SearchProducts?query=` | بحث منتجات (يُرجع JSON) | Admin, Cashier |
| GET | `/Sales/Edit/{id}` | نموذج تعديل فاتورة | Admin, Cashier |
| POST | `/Sales/Edit` | حفظ تعديل فاتورة | Admin, Cashier |
| POST | `/Sales/Delete/{id}` | حذف فاتورة (يُعيد المخزون) | Admin, Cashier |

### PurchasesController — `/Purchases`
| Method | Path | الوصف | الصلاحيات |
|--------|------|--------|-----------|
| GET | `/Purchases` | قائمة المشتريات | عام (مسجّل) |
| GET | `/Purchases/Create` | نموذج شراء جديد | عام |
| POST | `/Purchases/Create` | حفظ فاتورة شراء | عام |
| GET | `/Purchases/Details/{id}` | تفاصيل فاتورة | عام |
| GET | `/Purchases/Edit/{id}` | نموذج تعديل | عام |
| POST | `/Purchases/Edit/{id}` | حفظ تعديل | عام |
| POST | `/Purchases/Delete/{id}` | حذف فاتورة (يعكس المخزون) | عام |
| GET | `/Purchases/Search?q=` | بحث منتجات (JSON) | عام |

> ⚠️ **ملاحظة:** `PurchasesController` لا يوجد عليه `[Authorize]` صريح → لكن الوصول يتطلب تسجيل الدخول في الواقع لأن middleware الـ Authentication موجود. هذا قد يكون ثغرة أو قصداً.

### ProductsController — `/Products`
| Method | Path | الوصف | الصلاحيات |
|--------|------|--------|-----------|
| GET | `/Products` | قائمة المنتجات | Admin |
| GET | `/Products/Edit/{id}` | نموذج تعديل منتج | Admin |
| POST | `/Products/Edit` | حفظ تعديل منتج | Admin |
| GET | `/Products/Delete/{id}` | صفحة تأكيد الحذف | Admin |
| POST | `/Products/Delete/{id}` | تأكيد الحذف | Admin |

> **مهم:** لا توجد عملية Create للمنتجات عبر هذا Controller. المنتجات **تُضاف فقط عبر فاتورة الشراء** (PurchasesController).

### DashboardController — `/Dashboard`
| Method | Path | الوصف | الصلاحيات |
|--------|------|--------|-----------|
| GET | `/Dashboard` | لوحة التقارير الرئيسية (آخر 6 أشهر) | `[Authorize]` |
| GET | `/Dashboard/Today` | إحصاءات اليوم + رسوم ساعية | Admin |
| GET | `/Dashboard/Monthly?year=&month=` | إحصاءات شهر محدد | Admin |

### DebtsController — `/Debts`
| Method | Path | الوصف | الصلاحيات |
|--------|------|--------|-----------|
| GET | `/Debts` | قائمة الديون مجمّعة (مشتريات + مبيعات) | عام |
| GET | `/Debts/Details?name=&type=` | تفاصيل ديون عميل/مورد | عام |
| POST | `/Debts/Pay?id=&amount=` | تسجيل دفعة على فاتورة | عام |

### UsersController — `/Users`
| Method | Path | الوصف | الصلاحيات |
|--------|------|--------|-----------|
| GET | `/Users` | قائمة المستخدمين مع أدوارهم | Admin |
| GET | `/Users/Details/{id}` | تفاصيل مستخدم | Admin |
| GET | `/Users/Edit/{id}` | تعديل مستخدم | Admin |
| POST | `/Users/Edit` | حفظ التعديل (FullName + Role) | Admin |
| GET | `/Users/Create` | نموذج إنشاء كاشير | Admin |
| POST | `/Users/Create` | إنشاء كاشير | Admin |
| GET | `/Users/Delete/{id}` | صفحة تأكيد الحذف | Admin |
| POST | `/Users/Delete/{id}` | حذف مستخدم (يمنع حذف Admin) | Admin |

### CategoriesController — `/Categories`
| Method | Path | الوصف | الصلاحيات |
|--------|------|--------|-----------|
| GET | `/Categories` | قائمة التصنيفات | Admin |
| GET | `/Categories/Create` | نموذج إضافة تصنيف | Admin |
| POST | `/Categories/Create` | حفظ تصنيف | Admin |

### PricingController — `/Pricing`
| Method | Path | الوصف | الصلاحيات |
|--------|------|--------|-----------|
| GET | `/Pricing` | جدول أسعار جميع المنتجات | Admin |
| POST | `/Pricing/Update` | تحديث سعر البيع لمنتج | Admin |

### PricingSettingsController — `/PricingSettings`
| Method | Path | الوصف | الصلاحيات |
|--------|------|--------|-----------|
| GET | `/PricingSettings` | عرض إعدادات الربح والضريبة | Admin |
| POST | `/PricingSettings/Update` | تحديث نسبة الربح | Admin |

> ⚠️ **ملاحظة:** `Update` في `PricingSettingsController` يُحدّث `ProfitPercentage` فقط، لكن `TaxPercentage` في `PricingSetting` تُقرأ عند الحساب من كل مكان. لا يوجد endpoint لتعديل `TaxPercentage` عبر هذا Controller.

### InventoryController — `/Inventory`
| Method | Path | الوصف | الصلاحيات |
|--------|------|--------|-----------|
| GET | `/Inventory` | سجل حركة المخزون كاملاً | Admin |

### HomeController — `/`
| Method | Path | الوصف |
|--------|------|--------|
| GET | `/` أو `/Home` | لوحة تحكم رئيسية (ملخص سريع) |
| GET | `/Home/System` | صفحة الإعدادات (view فارغة تقريباً) |
| GET | `/Home/Privacy` | صفحة الخصوصية |
| GET | `/Home/Error` | صفحة الخطأ |

---

## 6. المتغيرات البيئية (Environment Variables)

المشروع **لا يستخدم `.env` files** — الإعدادات موجودة في:

### `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."   // ← سلسلة الاتصال بـ SQL Server
  },
  "Logging": { ... },
  "AllowedHosts": "*"
}
```

### `appsettings.Development.json`
```json
{
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" }
  }
}
```

### الإعدادات المطلوبة للتشغيل
| المتغير | الموقع | الغرض |
|---------|--------|-------|
| `ConnectionStrings:DefaultConnection` | `appsettings.json` | الاتصال بـ SQL Server |

**Connection String الحالية** (مخصصة لجهاز المطور):
```
Server=LAPAZM\SQLEXPRESS01;Database=alnakhilDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;
```
> يجب تعديلها لكل بيئة جديدة.

**ملاحظة:** لا توجد إعدادات للـ Email أو JWT أو Cloud — `EmailSender` هو stub فارغ.

---

## 7. الأوامر المهمة

### تشغيل المشروع محلياً
```bash
# من داخل مجلد المشروع (Alnkhil/)
dotnet run
# أو عبر Visual Studio: F5
```
> الـ URL الافتراضي: `https://localhost:7xxx` أو `http://localhost:5xxx` (حسب launchSettings.json)

### إدارة قاعدة البيانات
```bash
# إضافة migration جديد
dotnet ef migrations add [MigrationName]

# تطبيق الـ migrations على قاعدة البيانات
dotnet ef database update

# عرض قائمة الـ migrations
dotnet ef migrations list

# حذف آخر migration (قبل التطبيق)
dotnet ef migrations remove
```

### Build
```bash
dotnet build
dotnet publish -c Release -o ./publish
```

### لا توجد scripts في package.json
المشروع لا يستخدم Node.js — Frontend يعتمد على Razor + Bootstrap من `wwwroot/lib`.

### بيانات الدخول الافتراضية (Seed)
> **موجودة في `Data/DbInitializer.cs` لكن غير مستدعاة من Program.cs**
- Email: `admin@admin.com`
- Password: `Admin@123`
- دور: `Admin`

> المطور يجب أن يُنشئ حساب Admin يدوياً أو يستدعي `SeedRolesAndAdminAsync()` في Program.cs.

---

## 8. الميزات الحالية

### ✅ ميزات مُنفذة بالكامل

**إدارة المنتجات والمخزون:**
- عرض قائمة المنتجات مع التصنيف والكمية
- تعديل بيانات المنتج (اسم، باركود، أسعار، تصنيف، تاريخ انتهاء)
- حذف منتج
- سجل تاريخي لكل حركة مخزون (Purchase / Sale / Adjustment)
- منتجات تُضاف تلقائياً عند إنشاء فاتورة شراء لمنتج غير موجود

**إدارة الشراء:**
- إنشاء فاتورة شراء متعددة الأصناف
- دعم المنتجات الموجودة + إنشاء منتج جديد من داخل الفاتورة
- احتساب الضريبة والخصم على الفاتورة
- تحديث المخزون + حساب سعر البيع تلقائياً عند الشراء
- دعم الدفع الآجل (Deferred) مع تاريخ الاستحقاق
- تعديل وحذف فواتير الشراء

**إدارة البيع (POS):**
- إنشاء فاتورة بيع بالبحث عن المنتج بالاسم أو الباركود (AJAX)
- دعم الخصم على الفاتورة
- حالات الدفع: نقدي / آجل / معلّق
- الفواتير المعلّقة (Suspend): حفظ لاستكمالها لاحقاً بدون خصم من المخزون
- تعديل الفاتورة مع معالجة ذكية للمخزون (إضافة/إزالة/تعديل كميات)
- حذف فاتورة مع استعادة المخزون

**الديون:**
- عرض الديون مجمّعة حسب المورد أو العميل
- تفاصيل الديون لكل مورد/عميل
- تسجيل دفعات جزئية أو كاملة

**التقارير والإحصاءات:**
- لوحة تحكم رئيسية: مبيعات اليوم، عدد المنتجات، المنتجات منخفضة المخزون، المنتجات القريبة من الانتهاء، الديون
- رسم بياني لآخر 6 أشهر (مبيعات + مشتريات)
- إحصاءات يومية: مبيعات ساعة بساعة + أكثر 5 منتجات مبيعاً
- إحصاءات شهرية: مبيعات يوم بيوم + أكثر 5 منتجات + مقارنة بالشهر السابق

**إدارة المستخدمين:**
- عرض قائمة المستخدمين مع أدوارهم
- إنشاء حسابات كاشير
- تعديل الاسم والدور
- حذف المستخدمين (Admin محمي من الحذف)

**التسعير:**
- حساب سعر البيع تلقائياً: `roundUp(unitPrice × (1 + profit/100))` مرفوع دائماً لأعلى 100
- دعم السعر اليدوي للمنتج (`IsManualPrice = true`)
- صفحة مركزية لتعديل أسعار البيع
- إعدادات نسبة الربح والضريبة (جدول `PricingSetting` — صف واحد)

**التصنيفات:**
- إضافة وعرض تصنيفات المنتجات

### ⚠️ ميزات ناقصة أو نصف منفذة

1. **`EmailSender`:** Stub فارغ — لا يُرسل بريداً حقيقياً (مطلوب من Identity).
2. **`Home/System`:** صفحة الإعدادات تُرجع View فارغة تقريباً (لا يوجد محتوى فعلي).
3. **ملف الشخصي:** رابط "ملفي الشخصي" في القائمة الجانبية يشير إلى `#` (غير منفذ).
4. **`DbInitializer`:** موجود لكن غير مستدعى — لا يوجد Seed تلقائي للمدير.
5. **`DebtPayments` Table:** موجود في DbContext والـ Model لكن **لا يوجد Controller أو منطق** يضيف/يقرأ منه — `DebtsController.Pay` يُحدّث `AmountPaid` مباشرة على الفاتورة بدون تسجيل في `DebtPayments`.
6. **`Supplier` Model:** موجود مع جدول في DB، لكن لا يوجد Controller لإدارة الموردين. `SupplierName` يُحفظ كنص في `Purchase`.
7. **`SupplierId` في Purchase:** موجود في الـ Model لكن لا يُملأ من نموذج الإنشاء.
8. **تعديل `TaxPercentage` في PricingSettings:** `PricingSettingsController.Update` يُحدّث `ProfitPercentage` فقط، لا `TaxPercentage`.
9. **`PurchasesController` بدون `[Authorize]`:** قد يسمح للزوار غير المسجلين بالوصول.

---

## 9. نقاط يجب الانتباه لها

### ⚠️ Gotchas وقرارات غير بديهية

**1. `PurchasePrice` في `PurchaseItem` هو إجمالي السعر، ليس سعر الوحدة**
```csharp
// في PurchaseItem:
public decimal PurchasePrice { get; set; }  // إجمالي هذا الصنف
public decimal UnitPurchasePrice => Quantity == 0 ? 0 : PurchasePrice / Quantity; // سعر الوحدة محسوب
```
عند تحديث سعر البيع للمنتج في `InventoryService`:
```csharp
var unitPrice = item.PurchasePrice / item.Quantity; // سعر الوحدة
product.SalePrice = PricingHelper.CalculateSalePrice(unitPrice, profitPercentage);
```

**2. حساب سعر البيع: يُرفع دائماً للـ 100 التالية**
```csharp
return (Math.Floor(priceWithProfit / 100) + 1) * 100;
```
مثال: سعر شراء الوحدة 150، ربح 20% → 180 → يُقرَّب إلى **200** وليس 180.

**3. الفاتورة المعلّقة (Suspended) لا تخصم من المخزون**
```
IsSuspended = true + PaymentStatus = Suspended
InvoiceNumber = "H{N:D6}" (prefix H)
```
عند تحويلها لفاتورة عادية (Edit) تُولَّد رقم فاتورة جديد `"S{N:D6}"`.

**4. توليد رقم الفاتورة يتم في الذاكرة، غير Thread-Safe**
```csharp
// في SalesController:
private string GenerateInvoiceNumber(string prefix) { ... }
// يعتمد على Max(Id) — قد يتكرر في بيئة Concurrent
```

**5. Edit لفاتورة الشراء المدفوعة ممنوع**
```csharp
if (purchase.PaymentStatus == PaymentStatus.Paid) {
    ModelState.AddModelError("", "لا يمكن تعديل فاتورة مدفوعة");
    return View(vm);
}
```

**6. Delete فاتورة البيع يُعيد المخزون حتى للفواتير المعلّقة**
في `SalesController.Delete` يتم إعادة الكمية لكل Item — بما فيها المعلقة التي لم تُخصم أصلاً. قد يُسبب تضخماً في المخزون.

**7. `PurchaseVM` يحتوي حسابات `computed`، لكن القيم تُحفظ من الـ View**
```csharp
// PurchaseVM الـ properties computed:
public decimal Subtotal => Items.Sum(i => i.TotalPrice);
// لكن في Create POST:
purchase.Subtotal = vm.Subtotal; // يقرأ القيمة المحسوبة من الـ VM
```
الخطر: لو الـ View أرسلت قيمة مختلفة يمكن التلاعب بها.

**8. `appsettings.json` تحتوي اسم السيرفر الحقيقي**
```
Server=LAPAZM\SQLEXPRESS01
```
يجب تغييره عند النشر أو في بيئة مختلفة.

**9. Culture مُحددة في نهاية Program.cs (بعد `app.Build`)**
```csharp
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
```
هذا يضمن parsing الأرقام بالنقطة وليس الفاصلة.

### 🔴 ديون تقنية معروفة

1. **لا يوجد Transaction Scope** — عمليات الـ Save تتم في خطوات منفصلة في PurchasesController (SaveChanges مرتين).
2. **بعض الـ Controllers لا تُعيد ProductName عند الخطأ في Edit** — قد تفقد بيانات العرض.
3. **لا يوجد Unit Tests** أو Integration Tests.
4. **`TotalPrice` في `PurchaseItemVM`** مُعرَّفة كـ `=> PurchasePrice` بدون حساب (مضلّلة).
5. **`User.cs`** موجود في Models لكنه لا يُستخدم في أي مكان (أثر قديم).
6. **PurchasesController** بدون `[Authorize]` attribute.

---

## 10. خارطة سريعة للمطور / الذكاء الاصطناعي

### إذا أردت إضافة ميزة جديدة

| الميزة | من أين تبدأ |
|--------|------------|
| إضافة حقل جديد للمنتج | `Models/Product.cs` → ViewModel → Migration → View |
| إضافة تقرير جديد | `Controllers/DashboardController.cs` → `ViewModels/` → `Views/Dashboard/` |
| إضافة endpoint جديد | `Controllers/[اسم].cs` → View في `Views/[اسم]/` |
| تغيير منطق حساب السعر | `Helpers/PricingHelper.cs` |
| تغيير منطق المخزون | `Services/InventoryService.cs` |
| إضافة دور جديد | `Program.cs` (مصفوفة roles) + Controller attributes |
| تغيير layout أو القائمة الجانبية | `Views/Shared/_Layout.cshtml` |

### إذا أردت تعديل صفحة أو ميزة محددة

| الصفحة | Controller | View | ViewModel |
|--------|-----------|------|-----------|
| إنشاء فاتورة بيع | `SalesController.Create` | `Views/Sales/Create.cshtml` | `SaleVM` + `SaleItemVM` |
| قائمة المبيعات | `SalesController.Index` | `Views/Sales/Index.cshtml` | `List<Sale>` |
| إنشاء فاتورة شراء | `PurchasesController.Create` | `Views/Purchases/Create.cshtml` | `PurchaseVM` + `PurchaseItemVM` |
| لوحة التحكم الرئيسية | `HomeController.Index` | `Views/Home/Index.cshtml` | `DashboardVM` |
| التقارير | `DashboardController` | `Views/Dashboard/` | `DashboardVM`, `TodayStatsVM`, `MonthlyStatsVM` |
| الديون | `DebtsController` | `Views/Debts/` | `DebtSummaryVM`, `DebtDetailsVM`, `DebtVM` |
| إدارة المنتجات | `ProductsController` | `Views/Products/` | `ProductVM` |
| إدارة المستخدمين | `UsersController` | `Views/Users/` | `UserWithRoleVM`, `CreateCashierVM` |
| إعدادات التسعير | `PricingSettingsController` | `Views/PricingSettings/` | `PricingSetting` مباشرة |

### تدفق إضافة منتج جديد
1. المطور يذهب لـ `/Purchases/Create`
2. يختار "منتج جديد" بدل البحث عن موجود
3. يملأ الاسم والكمية والسعر
4. عند الحفظ: `PurchasesController.Create` ينشئ `Product` جديد → يحفظ → ثم `InventoryService.ApplyPurchaseAsync()` يُحدّث الكمية ويحسب سعر البيع

### تدفق عملية البيع
1. الكاشير يذهب لـ `/Sales/Create`
2. يبحث عن المنتج بالاسم أو الباركود (AJAX → `/Sales/SearchProducts`)
3. يضيف الكميات
4. يختار حالة الدفع (نقدي/آجل/معلّق)
5. عند الحفظ: يُحتسب `Subtotal + TaxAmount - Discount = Total`
6. ثم `InventoryService.ApplySaleAsync()` يخصم الكميات ويسجل حركة المخزون

---

## ملاحظات على التناقضات الموجودة في الكود

1. **`DashboardController` يستخدم `[Authorize]` عام** بينما `DashboardController.Today` و `Monthly` ليس عليهم attribute خاص — يرثان من Class-level `[Authorize]`.

2. **`HomeController.Index`** يحسب `LowStockCount` بـ `Quantity < 50` بينما **`DashboardController.Index`** يستخدم `Quantity <= 5` — تناقض في تعريف "منخفض المخزون".

3. **`DbInitializer`** غير مستدعى من `Program.cs` — يوجد كود مكرر في `Program.cs` لإنشاء الأدوار.

4. **`DebtPayments` Table** موجودة في قاعدة البيانات وفي DbContext لكن لا يُكتب فيها — `DebtsController.Pay` يُحدّث الفاتورة مباشرة.

5. **`PurchasesController.Edit`** عند التعديل يُضيف كميات للمخزون مباشرة بدون التحقق من الكميات القديمة (لا يُزيل الكميات القديمة أولاً).

---

*آخر تحديث لهذا الملف: بناءً على قراءة الكود الفعلي — أغسطس 2026*
