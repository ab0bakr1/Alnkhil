# PROJECT_CONTEXT.md — النخيل Smart POS
> توثيق شامل ومُحدّث للمشروع — مُعدّ للقراءة من قِبل أي نموذج ذكاء اصطناعي أو مطور جديد.

---

## 1. نظرة عامة

### ما هو المشروع؟
**النخيل Smart POS** هو نظام نقطة بيع (Point of Sale) وإدارة سوبر ماركت متكامل. يغطّي دورة العمل الكاملة:
الشراء وإدارة الموردين ← إدارة المخزون والتسعير ← البيع للعملاء وإدارة الكاشير ← تتبع الديون (عملاء وموردين) ودفوعاتها ← التقارير والإحصاءات التحليلية.

### من هم المستخدمون؟
- **Admin (مدير):** صلاحيات كاملة — إدارة المنتجات، التصنيفات، الموردين، المستخدمين والكاشيرين، سجل المخزون، إعدادات التسعير، فواتير الشراء، حذف فواتير البيع، والتقارير التحليلية المتقدمة.
- **Cashier (كاشير):** صلاحيات تشغيلية — إنشاء وتعديل فواتير البيع، الفواتير المعلقة، البحث عن المنتجات، عرض الديون وتسجيل السدادات، وعرض قائمة المشتريات. (تم منع حذف فواتير البيع أو تعديل/حذف فواتير الشراء للموظفين العاديين).

### نوع المشروع
نظام ERP/POS داخلي (Internal Business Tool) مبني على ASP.NET Core 8.0 MVC مع واجهات Razor مخصصة، قابل للنشر السحابي أو الاستخدام المحلي.

### حالة المشروع
**الإصدار الحالي: V 2.1 (سبتمبر 2026)**
- تم تطبيق تحسينات جذرية في إصداري V 2.0 و V 2.1 (إضافة إدارة كاملة للموردين، ربط جدول تسجيل الدفعات `DebtPayments`، دعم تعديل الضريبة والربح، إصلاح تتبع المخزون عند تعديل المشتريات، ضبط الصلاحيات، وإضافة إعدادات الإنتاج `appsettings.Production.json`).
- تم تجهيز قاعدة بيانات إنتاجية على استضافة سحابية (`site4now.net`).

---

## 2. Tech Stack

### اللغات والـ Frameworks
| الطبقة | التقنية | التفاصيل |
|--------|---------|----------|
| **Backend** | ASP.NET Core 8.0 (MVC) | C# (.NET 8.0) |
| **Frontend** | Razor Views (.cshtml) + Vanilla JS + Bootstrap 5 RTL | واجهات عربية تفاعلية بالكامل |
| **Database** | SQL Server via Entity Framework Core 8.0 | Code-First مع Migrations |
| **Auth** | ASP.NET Core Identity | إدارة المستخدمين والأدوار (Admin / Cashier) |
| **Culture** | `en-US` | مثبتة في `Program.cs` لضمان صحة الأرقام العشرية (Decimal Parsing) |

### المكتبات الأساسية (NuGet Packages)
| المكتبة | الإصدار | الغرض |
|---------|---------|-------|
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 8.0.0 | مصادقة وأذونات وجداول Identity |
| `Microsoft.AspNetCore.Identity.UI` | 8.0.0 | صفحات المصادقة التلقائية |
| `Microsoft.EntityFrameworkCore` | 8.0.0 | ORM الرئيسي |
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.0 | مزود الاتصال بقاعدة بيانات SQL Server |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.0 | أدوات الـ CLI والـ Migrations |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | 8.0.0 | أدوات التوليد التلقائي (Scaffolding) |

### مكونات الواجهة (Frontend)
- **محلياً (`wwwroot/lib`):** Bootstrap 5 RTL، jQuery، Bootstrap Icons
- **CDN:** خط `Cairo` من Google Fonts، أيقونات Bootstrap Icons CDN
- **صور وتصاميم:** صور رمزية للمستخدمين عبر UI-Avatars API

### البيئة والنشر
- **قاعدة البيانات المحلية / التطويرية:** SQL Server Local Instance (`LAPAZM\SQLEXPRESS01`).
- **قاعدة بيانات الإنتاج السحابية:** SQL Server على `sql6031.site4now.net` (معرّفة في `appsettings.Production.json` و `appsettings.json`).
- **تكوين الـ JSON:** `JsonOptions.PropertyNamingPolicy = null` مفعل لضمان تطابق أسماء الخصائص (PascalCase) بين C# و Javascript.

---

## 3. هيكل المجلدات الفعلي

```
Alnkhil/
├── Areas/
│   └── Identity/
│       └── Pages/
│           └── Account/             ← صفحات Login/Logout الجاهزة
├── Controllers/                     ← المتحكمات (Controllers)
│   ├── CategoriesController.cs      ← إدارة التصنيفات (Admin)
│   ├── DashboardController.cs       ← التقارير التحليلية واليومية والشهرية
│   ├── DebtsController.cs           ← إدارة الديون وسدادها (Admin + Cashier)
│   ├── HomeController.cs            ← لوحة القيادة السريعة وبوابة النظام (System)
│   ├── InventoryControllers.cs      ← سجل حركات المخزون التاريخية (Admin)
│   ├── PricingController.cs         ← جدول مراقبة وتعديل أسعار البيع وهوامش الربح (Admin)
│   ├── PricingSettingsController.cs ← إعدادات نسبة الربح ونسبة الضريبة (Admin)
│   ├── ProductsController.cs        ← عرض وتعديل وحذف المنتجات (Admin)
│   ├── PurchasesController.cs       ← فواتير الشراء والطلبيات والبحث (Admin + Cashier)
│   ├── SalesController.cs           ← فواتير البيع ونقاط البيع والفواتير المعلقة (Admin + Cashier)
│   ├── SuppliersController.cs       ← [جديد V2.0] إدارة الموردين وبيانات الاتصال (Admin)
│   └── UsersController.cs           ← إدارة الكاشيرين وصلاحيات المستخدمين (Admin)
├── Data/
│   ├── alnakhilContext.cs           ← DbContext الرئيسي متضمناً كافة الـ DbSets
│   └── DbInitializer.cs             ← تهيئة الأدوار والمستخدم الافتراضي (Admin)
├── Helpers/
│   └── PricingHelper.cs             ← خوارزمية تسعير البيع التلقائي بالتقريب للـ 100
├── Migrations/                      ← 61 ملف migrations (30 migration + ModelSnapshot)
├── Models/                          ← كيانات وقواعد البيانات (Entities)
│   ├── ApplicationUser.cs           ← توسيع لـ IdentityUser (مع حقل FullName)
│   ├── Category.cs                  ← تصنيف الأصناف
│   ├── DebtPayment.cs               ← سجل سداد الديون الجزئية والكلية
│   ├── ErrorViewModel.cs            ← نموذج رسائل الخطأ
│   ├── InventoryTransaction.cs      ← سجل حركة المخزن (Purchase, Sale, Adjustment)
│   ├── PaymentStatus.cs             ← Enum: Unpaid, Paid, Deferred, Suspended
│   ├── PricingSetting.cs            ← إعدادات الربح والضريبة
│   ├── Product.cs                   ← بيانات المنتج والباركود وسعر الشراء والبيع
│   ├── Purchase.cs                  ← ترويسة فاتورة الشراء وبيانات المورد
│   ├── PurchaseItem.cs              ← أصناف فاتورة الشراء
│   ├── Sale.cs                      ← ترويسة فاتورة البيع والعميل وحالة التعليق
│   ├── SaleItem.cs                  ← أصناف فاتورة البيع وأسعار البيع الفعلية
│   ├── Supplier.cs                  ← بيانات المورد (الاسم، الهاتف، وقائمة مشترياته)
│   └── User.cs                      ← (ملف قديم غير مستخدم)
├── Properties/
│   └── launchSettings.json          ← ملف إعدادات التشغيل
├── Services/
│   ├── EmailSender.cs               ← Stub لـ IEmailSender
│   └── InventoryService.cs          ← تطبيق العمليات على المخزون (شراء، بيع، إلغاء)
├── ViewModels/                      ← نماذج العرض وتمرير البيانات (DTOs)
│   ├── CreateCashierVM.cs
│   ├── DashboardVM.cs
│   ├── DebtDetailsVM.cs
│   ├── DebtSummaryVM.cs
│   ├── DebtVM.cs
│   ├── MonthlyStatsVM.cs
│   ├── PricingVM.cs
│   ├── ProductVM.cs
│   ├── PurchaseItemVM.cs
│   ├── PurchaseVM.cs (يتضمن SupplierId)
│   ├── SaleItemVM.cs
│   ├── SaleVM.cs
│   ├── TodayStatsVM.cs
│   └── UserWithRoleVM.cs
├── Views/                           ← واجهات Razor
│   ├── Categories/                  ← Index, Create
│   ├── Dashboard/                   ← Index, Today, Monthly
│   ├── Debts/                       ← Index, Details, _DebtTable
│   ├── Home/                        ← Index, System (بوابة الإدارة), Privacy
│   ├── Inventory/                   ← Index (سجل الحركات)
│   ├── Pricing/                     ← Index (مراقبة وتعديل الأسعار)
│   ├── PricingSettings/             ← Index (تعديل نسب الربح والضريبة)
│   ├── Products/                    ← Index, Edit, Delete
│   ├── Purchases/                   ← Index, Create, Edit, Details
│   ├── Sales/                       ← Index, Create, Edit, Details
│   ├── Shared/                      ← _Layout, _LoginPartial, _ValidationScriptsPartial
│   ├── Suppliers/                   ← [جديد V2.0] Index, Create, Edit
│   ├── Users/                       ← Index, Create, Edit, Delete, Details
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/                         ← الملفات الثابتة (CSS, JS, Libs)
├── alnakhil.csproj                  ← ملف إعدادات المشروع وحزم NuGet
├── alnakhil.sln                     ← ملف الحل (Solution)
├── appsettings.json                 ← إعدادات السيرفر والاتصال
├── appsettings.Development.json     ← إعدادات بيئة التطوير
├── appsettings.Production.json      ← [جديد V2.1] إعدادات بيئة الإنتاج السحابية
└── Program.cs                       ← نقطة الدخول، إعداد الخدمات، وتهيئة قاعدة البيانات
```

---

## 4. المعمارية (Architecture) وقواعد البيانات

### نمط التصميم
- **ASP.NET Core MVC (Server-Side Rendering)** مع Razor Views.
- يتم التواصل التفاعلي في شاشات البيع والشراء عبر مكالمات AJAX داخلية لإرجاع كائنات JSON (مثل البحث عن المنتجات بالباركود أو الاسم، أو تعليق الفواتير).

### هيكل قاعدة البيانات والعلاقات

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
  ├── PurchasePrice (decimal 18,2)   ← سعر شراء الوحدة الواحدة (Unit Price)
  ├── SalePrice (decimal 18,2)       ← سعر البيع المقترح أو اليدوي
  ├── IsManualPrice (bool)           ← هل تم تحديد السعر يدوياً
  ├── ManualSalePrice (decimal?)     ← السعر اليدوي المحفوظ
  ├── ExpirationDate (DateTime?)
  └── CategoryId (FK → Category, nullable)

Supplier
  ├── Id (PK)
  ├── Name (Required)
  ├── Phone (string?)
  └── Purchases (Navigation 1:N → Purchase)

Purchase
  ├── Id (PK)
  ├── PurchaseDate (DateTime)
  ├── InvoiceNumber (string?, max 50) ← "PUR-{Id:00000}"
  ├── SupplierName (string, max 150)
  ├── SupplierId (FK → Supplier, nullable) ← [مفعل V2.0]
  ├── Subtotal / TaxPercentage / TaxAmount / DiscountPercentage / DiscountAmount / TotalAmount
  ├── AmountPaid (decimal)
  ├── PaymentStatus (enum: Paid, Deferred, Unpaid)
  └── DueDate (DateTime?)             ← إلزامي عند اختيار دين آجل (Deferred)

PurchaseItem
  ├── Id (PK)
  ├── PurchaseId (FK → Purchase)
  ├── ProductId (FK → Product)
  ├── CategoryId (FK → Category, nullable)
  ├── Quantity (int)
  ├── PurchasePrice (decimal)         ← إجمالي سعر هذا الصنف بالفاتورة
  ├── ExpiryDate (DateTime?)
  └── [NotMapped] UnitPurchasePrice = PurchasePrice / Quantity

Sale
  ├── Id (PK)
  ├── SaleDate (DateTime)
  ├── InvoiceNumber (Required)        ← "S{N:D6}" عادي أو "H{N:D6}" معلّق
  ├── CustomerName (string?)
  ├── Subtotal / TaxAmount / DiscountAmount / TotalAmount
  ├── AmountPaid (decimal)
  ├── IsSuspended (bool)
  ├── PaymentStatus (enum: Paid, Deferred, Unpaid, Suspended)
  └── DueDate (DateTime?)

SaleItem
  ├── Id (PK)
  ├── SaleId (FK → Sale)
  ├── ProductId (FK → Product)
  ├── Quantity (int)
  └── UnitPrice (decimal)

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
  ├── ProfitPercentage (decimal)      ← نسبة الربح الافتراضية
  └── TaxPercentage (decimal)         ← نسبة الضريبة الافتراضية (القيمة الافتراضية: 0%)
```

#### العلاقات المترابطة:
- `Supplier (1)` ⟷ `(N) Purchase`: يمكن اختيار مورد مسجل أو كتابة اسم مورد جديد ليتم إنشاؤه تلقائياً.
- `Purchase (1)` ⟷ `(N) PurchaseItem` ⟷ `(1) Product`
- `Sale (1)` ⟷ `(N) SaleItem` ⟷ `(1) Product`
- `Product (N)` ⟷ `(1) Category`
- `Purchase (1)` ⟷ `(N) DebtPayment`: يتم إنشاء سجل دفع فعلي في جدول `DebtPayments` عند كل عملية سداد لمورد.
- `Sale (1)` ⟷ `(N) DebtPayment`: يتم إنشاء سجل دفع فعلي في جدول `DebtPayments` عند كل عملية سداد من عميل.

### نظام المصادقة والـ Seed التلقائي
1. **التهيئة التلقائية (`DbInitializer`):**
   - يتم استدعاؤه تلقائياً في `Program.cs` عند إقلاع التطبيق عبر `DbInitializer.SeedRolesAndAdminAsync()`.
   - يقوم بإنشاء دوري `Admin` و `Cashier`.
   - يقوم بإنشاء حساب المدير الافتراضي إذا لم يكن موجوداً:
     - **Email:** `admin@admin.com`
     - **Password:** `Admin@123`
     - **Role:** `Admin`
2. **إعدادات كلمات المرور:**
   - مخففة لسهولة الاستخدام بنقاط البيع (`RequireDigit = false`, `RequiredLength = 6`, `RequireUppercase = false`, `RequireNonAlphanumeric = false`).

---

## 5. خريطة المتحكمات والـ Endpoints

### 1. SuppliersController — `/Suppliers` [جديد V2.0]
| الـ Method | المسار | الوصف | الصلاحيات المطلوبة |
|------------|--------|--------|---------------------|
| GET | `/Suppliers` | قائمة الموردين مع أرقام الهواتف وعدد فواتيرهم | `Admin` |
| GET | `/Suppliers/Create` | نموذج إضافة مورد جديد | `Admin` |
| POST | `/Suppliers/Create` | حفظ المورد مع منع تكرار الأسماء | `Admin` |
| GET | `/Suppliers/Edit/{id}` | نموذج تعديل بيانات المورد | `Admin` |
| POST | `/Suppliers/Edit/{id}` | حفظ تعديلات اسم وهاتف المورد | `Admin` |

### 2. PurchasesController — `/Purchases`
| الـ Method | المسار | الوصف | الصلاحيات المطلوبة |
|------------|--------|--------|---------------------|
| GET | `/Purchases` | استعراض فواتير الشراء | `Admin, Cashier` |
| GET | `/Purchases/Create` | نموذج إنشاء فاتورة شراء جديدة وتحديد المورد وحالة الدفع | مسجل |
| POST | `/Purchases/Create` | حفظ الفاتورة، ربط/إنشاء المورد، منع تكرار المنتجات بالاسم، وتحديث المخزون | مسجل |
| GET | `/Purchases/Details/{id}`| تفاصيل الفاتورة وأصنافها والمورد | مسجل |
| GET | `/Purchases/Edit/{id}` | نموذج تعديل الفاتورة (يمنع تعديل الفواتير المدفوعة بالكامل) | `Admin` |
| POST | `/Purchases/Edit/{id}` | حفظ التعديل مع **عكس كميات الأصناف القديمة من المخزون أولاً** قبل تطبيق الأصناف المعدلة | مسجل (يمنع Paid) |
| POST | `/Purchases/Delete/{id}`| حذف الفاتورة وإلغاء الكميات من المخزون | `Admin` |
| GET | `/Purchases/Search?q=` | بحث سريع بالاسم والباركود لإكمال أصناف الفاتورة (JSON) | مسجل |

### 3. SalesController — `/Sales`
| الـ Method | المسار | الوصف | الصلاحيات المطلوبة |
|------------|--------|--------|---------------------|
| GET | `/Sales` | قائمة جميع فواتير البيع | `Admin, Cashier` |
| GET | `/Sales/Details/{id}` | تفاصيل الفاتورة وطباعتها | `Admin, Cashier` |
| GET | `/Sales/Create` | شاشة نقطة البيع (POS) | `Admin, Cashier` |
| POST | `/Sales/Create` | حفظ الفاتورة وخصم الكميات من المخزن | `Admin, Cashier` |
| POST | `/Sales/Suspend` | تعليق الفاتورة (`IsSuspended = true`) بدون خصم كميات | `Admin, Cashier` |
| GET | `/Sales/SearchProducts?query=` | بحث المنتجات والباركود في شاشة البيع (JSON) | `Admin, Cashier` |
| GET | `/Sales/Edit/{id}` | فتح وتعديل فاتورة أو استئناف فاتورة معلقة | `Admin, Cashier` |
| POST | `/Sales/Edit` | حفظ التعديلات وإدارة فروقات المخزون بذكاء | `Admin, Cashier` |
| POST | `/Sales/Delete/{id}` | حذف فاتورة البيع واسترجاع كمياتها للمخزن | **`Admin` فقط** |

### 4. DebtsController — `/Debts`
| الـ Method | المسار | الوصف | الصلاحيات المطلوبة |
|------------|--------|--------|---------------------|
| GET | `/Debts` | جدول ملخص ديون الموردين (علينا) والعملاء (لنا) | `Admin, Cashier` |
| GET | `/Debts/Details?name=&type=` | كشف حساب تفصيلي لعميل أو مورد وطباعة التقرير | `Admin, Cashier` |
| POST | `/Debts/Pay` | سداد دفعة وتحديث حالة الفاتورة **وتسجيل السداد في جدول `DebtPayments`** | `Admin, Cashier` |

### 5. PricingSettingsController — `/PricingSettings`
| الـ Method | المسار | الوصف | الصلاحيات المطلوبة |
|------------|--------|--------|---------------------|
| GET | `/PricingSettings` | عرض إعدادات نسبة الربح ونسبة الضريبة | `Admin` |
| POST | `/PricingSettings/Update` | تحديث **نسبة الربح ونسبة الضريبة** معاً | `Admin` |

### 6. PricingController — `/Pricing`
| الـ Method | المسار | الوصف | الصلاحيات المطلوبة |
|------------|--------|--------|---------------------|
| GET | `/Pricing` | جدول مقارنة تكلفة آخر شراء وسعر البيع وهوامش الربح | `Admin` |
| POST | `/Pricing/Update` | تعديل سعر البيع وتثبيته كسعر يدوي (`IsManualPrice = true`) | `Admin` |

### 7. DashboardController — `/Dashboard`
| الـ Method | المسار | الوصف | الصلاحيات المطلوبة |
|------------|--------|--------|---------------------|
| GET | `/Dashboard` | لوحة المؤشرات الشاملة، آخر 6 أشهر، والديون والمخزون المنخفض | `[Authorize]` |
| GET | `/Dashboard/Today` | حركة مبيعات اليوم الساعية وأعلى 5 منتجات مبيعاً | `[Authorize]` |
| GET | `/Dashboard/Monthly` | مبيعات ومشتريات الشهر المختار يومياً ومقارنتها بالسابق | `[Authorize]` |

### 8. ProductsController — `/Products`
| الـ Method | المسار | الوصف | الصلاحيات المطلوبة |
|------------|--------|--------|---------------------|
| GET | `/Products` | قائمة كافة منتجات المخزن مع الكميات والأسعار | `Admin` |
| GET | `/Products/Edit/{id}` | تعديل بيانات الصنف والتسعير وتاريخ الانتهاء | `Admin` |
| POST | `/Products/Edit` | حفظ تعديل الصنف | `Admin` |
| GET | `/Products/Delete/{id}` | صفحة تأكيد الحذف | `Admin` |
| POST | `/Products/Delete/{id}` | حذف المنتج نهائياً من النظام | `Admin` |

### 9. UsersController — `/Users`
| الـ Method | المسار | الوصف | الصلاحيات المطلوبة |
|------------|--------|--------|---------------------|
| GET | `/Users` | قائمة المستخدمين وأدوارهم وهواتفهم | `Admin` |
| GET | `/Users/Create` | نموذج إنشاء حساب كاشير جديد | `Admin` |
| POST | `/Users/Create` | حفظ وإنشاء الكاشير وإسناد دور Cashier له | `Admin` |
| GET | `/Users/Edit/{id}` | تعديل بيانات وصلاحية المستخدم | `Admin` |
| POST | `/Users/Edit` | حفظ الاسم وتغيير الدور | `Admin` |
| POST | `/Users/Delete/{id}` | حذف مستخدم (محمي: يمنع حذف الأدمن) | `Admin` |

### 10. Controllers إضافية
- **`CategoriesController` (`/Categories`):** إضافة واستعراض أقسام المنتجات (Admin).
- **`InventoryControllers` (`/Inventory`):** استعراض تاريخ حركات التوريد والبيع والتسوية (Admin).
- **`HomeController` (`/`):**
  - `/` أو `/Home/Index`: لوحة مؤشرات سريعة ومختصرة.
  - `/Home/System`: بوابة الإدارة والنظام المركزية (روابط سريعة لكافة أقسام المنظومة).

---

## 6. الإعدادات وسلاسل الاتصال (Configuration)

### ملفات الإعداد
1. **`appsettings.json`:**
   يحتوي على إعدادات الاتصال الافتراضية ومستوى السجلات:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=sql6031.site4now.net;Initial Catalog=db_ace006_alnkhil;User Id=db_ace006_alnkhil_admin;Password=@Azm210699;Encrypt=True;TrustServerCertificate=True;"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```
2. **`appsettings.Production.json`:**
   يحتوي على سلسلة الاتصال المخصصة لبيئة الإنتاج السحابي على خادم `site4now.net`.
3. **`appsettings.Development.json`:**
   مخصص لمستوى سجلات بيئة التطوير المحلية.

---

## 7. منطق الأعمال المتقدم (Business Logic & Gotchas)

### 1. تسعير الشراء والبيع والوحدات
- **في `PurchaseItem`:** خاصية `PurchasePrice` تمثل **إجمالي سعر الصنف** في الفاتورة (مثال: سعر الكرتون كاملاً)، وسعر الوحدة الواحدة يُحسب عبر:
  $$\text{UnitPurchasePrice} = \frac{\text{PurchasePrice}}{\text{Quantity}}$$
- **في `Product`:** خاصية `PurchasePrice` في جدول المنتجات تُخزن **سعر شراء الوحدة الواحدة** (`unitPrice`) وليس إجمالي سعر الكرتون (تم توحيد هذا المنطق في `InventoryService`).
- **حساب سعر البيع التلقائي (`PricingHelper`):**
  يتم احتساب السعر بإضافة نسبة الربح مع **رفعه دائماً إلى أقرب 100 تالية**:
  $$\text{SalePrice} = \left(\lfloor \frac{\text{PriceWithProfit}}{100} \rfloor + 1\right) \times 100$$
  *مثال:* تكلفة الشراء 150 مع ربح 20% = 180 ← يُرفع تلقائياً إلى **200**.
- **السعر اليدوي (`IsManualPrice`):** عند تعديل السعر من صفحة `Pricing` أو `Products/Edit` يتم تثبيت `IsManualPrice = true` حتى لا يتم تغييره تلقائياً عند شراء شحنات لاحقة إلا إذا أُلغي التثبيت.

### 2. معالجة المخزون الدقيقة عند تعديل فاتورة الشراء (`PurchasesController.Edit`)
- عند تعديل فاتورة شراء قديمة، يقوم النظام أولاً **بعكس وحذف الكميات القديمة من المخزن** وتسجيل حركة مخزون سالبة، ثم يحذف الأصناف القديمة ويضيف الأصناف الجديدة مع تحديث المخزن، مما يمنع تراكم الكميات الوهمي.

### 3. الفواتير المعلقة (`Suspended Sales`)
- فواتير البيع المعلقة تأخذ البادئة `H` (مثل `H000012`) وحالة `PaymentStatus = Suspended`.
- الفواتير المعلقة **لا تخصم من المخزون** حتى يتم استئنافها وتأكيد بيعها فتتحول إلى `S` (مثل `S000012`).

### 4. حماية حركة المبيعات والمشتريات
- حذف فواتير البيع مقصور على الـ `Admin` فقط.
- تعديل فواتير الشراء مقصور على الـ `Admin` وممنوع تماماً على أي فاتورة حالتها `Paid`.

### 5. نظام الديون والسداد (`Debts & DebtPayments`)
- كل عملية سداد تتم عبر `DebtsController.Pay` تقوم بتسجيل صف جديد في جدول `DebtPayments` وتعديل المبلغ المسدد `AmountPaid` على الفاتورة.
- إذا أصبح `AmountPaid >= TotalAmount` تتحول الفاتورة تلقائياً إلى `Paid`، وإلا تظل `Deferred`.

---

## 8. دليل المطور السريع (Developer Cheat Sheet)

### لتشغيل وتطوير المشروع
```bash
# بناء المشروع
dotnet build

# تشغيل المشروع محلياً
dotnet run

# إضافة Migration جديد عند تعديل النماذج
dotnet ef migrations add [اسم_التعديل]

# تطبيق التعديلات على قاعدة البيانات
dotnet ef database update
```

### بيانات الدخول التلقائية للمدير
- **البريد:** `admin@admin.com`
- **كلمة المرور:** `Admin@123`
- **الدور:** `Admin`

### أين تجد الكود المطلوب لتعديل الميزات؟
| المطلوب تعديله | الملفات المسؤولة |
|----------------|------------------|
| تعديل بيانات أو شاشات الموردين | [SuppliersController.cs](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Controllers/SuppliersController.cs) + [Views/Suppliers/](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Views/Suppliers/) |
| تعديل نسب الربح والضريبة الافتراضية | [PricingSettingsController.cs](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Controllers/PricingSettingsController.cs) + [Views/PricingSettings/](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Views/PricingSettings/) |
| تعديل مراقبة الأسعار وهامش الربح | [PricingController.cs](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Controllers/PricingController.cs) + [Views/Pricing/](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Views/Pricing/) |
| شاشة البيع وحسابات الكاشير | [SalesController.cs](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Controllers/SalesController.cs) + [Views/Sales/Create.cshtml](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Views/Sales/Create.cshtml) |
| شاشة الشراء وإضافة البضاعة | [PurchasesController.cs](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Controllers/PurchasesController.cs) + [Views/Purchases/Create.cshtml](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Views/Purchases/Create.cshtml) |
| كشوفات الديون والسداد | [DebtsController.cs](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Controllers/DebtsController.cs) + [Views/Debts/](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Views/Debts/) |
| القائمة الجانبية والهيدر العام | [Views/Shared/_Layout.cshtml](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Views/Shared/_Layout.cshtml) |
| منطق تأثير المخزن | [Services/InventoryService.cs](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Services/InventoryService.cs) |
| خوارزمية التسعير التقريبية | [Helpers/PricingHelper.cs](file:///c:/Users/aboba/OneDrive/Desktop/Alnkhil/Helpers/PricingHelper.cs) |

---

*تاريخ التحديث الأخير لهذا الملف: سبتمبر 2026 — متوافق كلياً مع الإصدارين V2.0 و V2.1 والكود المصدري الفعلي للمشروع.*
