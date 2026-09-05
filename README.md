# Invoicing App

## Architektúra és rétegződés

A projekt a **Clean Architecture / Rétegelt architektúra** elveit követi, egyetlen Solutionbe rendezve, négy jól elkülönülő projekttel:

* **Invoicing.Domain:** A legbelső mag; kizárólag az üzleti entitásokat (`Product`, `Customer`, `Order`, `OrderItem`) tartalmazza. Nem függ semmilyen külső könyvtártól vagy keretrendszertől.
* **Invoicing.Application:** Az üzleti logikát fogja össze. Itt találhatók a szolgáltatások (`Services`), a be- és kimeneti modellek (`DTO-k`), valamint az adatelérési és dokumentumgenerálási interfészek (`Interfaces`).
* **Invoicing.Infrastructure:** A technikai részletek megvalósítása. Itt kapott helyet az EF Core `AppDbContext`, a migrációk, a `Repository` és `UnitOfWork` implementációk, valamint a `QuestPDF` dokumentumgenerátor.
* **Invoicing.Api:** A belépési pont. ASP.NET Core Web API vezérlők (`Controllers`), Swagger konfiguráció és a Dependency Injection (DI) bekötések.

### Miért ezt az architektúrát választottam?

* **Felelősségek szétválasztása (Separation of Concerns):** A webes végpontok semmit sem tudnak az adatbázis fizikai működéséről, a Domain entitások pedig teljesen függetlenek az API vagy az ORM technikai részleteitől.
* **Függőség-megfordítás elve (DIP - SOLID):** Az üzleti logika rétege (`Application`) határozza meg a szükséges műveletek szerződését (pl. `IOrderRepository`), míg a külső réteg (`Infrastructure`) csupán implementálja azokat. Ez biztosítja az egyszerű cserélhetőséget és a kényelmes tesztelhetőséget (mockolhatóságot) izolált környezetben.

---

## Futtatás

### Előfeltételek

* [.NET 10 SDK](https://dotnet.microsoft.com/)

### Lépések

1. Klónozd a repository-t:
   ```bash
   git clone <repo-url>
   cd Invoicing
   ```

2. Indítsd el az alkalmazást:
   ```bash
   dotnet run --project src/Invoicing.Api
   ```

> Az SQLite adatbázis (`invoicing.db`) és a séma automatikusan létrejön az indításkor a `Program.cs`-ben konfigurált `db.Database.Migrate()` hívás révén, feltöltve a teszteléshez szükséges kezdőadatokkal (Seed data).

### Elérhetőség

* **Swagger UI:** `https://localhost:<port>/swagger` (a futtatási port a konzol kimenetén látható).

---

## Fő API végpontok

| Metódus | Végpont | Leírás |
| :--- | :--- | :--- |
| `GET` | `/api/products` | Termékek listázása |
| `GET` | `/api/products/{id}` | Egy termék részletei |
| `POST` | `/api/products` | Új termék rögzítése |
| `GET` | `/api/customers` | Ügyfelek listázása |
| `GET` | `/api/customers/{id}` | Egy ügyfél részletei |
| `POST` | `/api/customers` | Új ügyfél rögzítése |
| `GET` | `/api/orders` | Rendelések listázása a hozzájuk tartozó tételekkel |
| `GET` | `/api/orders/{id}` | Egy rendelés részletei |
| `POST` | `/api/orders` | Új rendelés rögzítése tételek megadásával |
| `GET` | `/api/orders/{id}/invoice` | A megadott azonosítójú rendelés számlájának letöltése formázott PDF fájlként |

---

## Tesztelés

A `tests/Invoicing.Application.Tests` projekt xUnit + Moq alapú unit teszteket tartalmaz, elsősorban az `OrderService` üzleti logikájára (validációs hibaágak, sikeres rendelés-létrehozás, számla-generálás) és a mapping extension metódusokra fókuszálva — ez a réteg tartalmazza a legtöbb elágazó logikát.

Futtatás:
```bash
dotnet test
```

---

## SQL mellékletek

A feladat által elvárt nyers SQL állományok a `docs/` mappában érhetők el:

* `docs/schema.sql` — A táblák létrehozásához szükséges utasítások és a mintaadatok.
* `docs/queries.sql` — A két kötelező elemző lekérdezés:
  1. Top 3 termék a rendelt darabszám alapján.
  2. Rendelések, amelyek legalább egy veszélyes (`IsHazardous`) terméket tartalmaznak.

---

## Tervezési döntések és megfontolások

### 1. Törékeny termékek kezelése (`IsFragile` mező bevezetése)
A leírás szövege a törékeny termékek egyedi számlasori jelölését írja elő, míg a megadott mezőlistában csak az `IsHazardous` szerepelt. Mivel a veszélyesség és a törékenység különálló terméktulajdonság (és a kiírásban szereplő SQL lekérdezés explicit a veszélyes termékekre kérdez rá), nem vontam össze a kettőt. A feladat által jelzett hiányzó mezők feloldásaként egy külön `IsFragile` (`bool`) tulajdonságot vezettem be a `Product` entitáson.

### 2. Külön Service réteg még az egyszerű olvasásokhoz is
Annak ellenére, hogy a termékek vagy ügyfelek listázásakor könnyebb lett volna a Repository-t közvetlenül a Controllerben meghívni, minden entitás saját szolgáltatást kapott (`ProductService`, `CustomerService`):
* **Konzisztens architektúra:** A vezérlők kizárólag a HTTP kommunikációért (státuszkódok, bemeneti validáció, routing) felelnek.
* **DTO absztrakció:** Az entitások DTO-kká alakítása az alkalmazás rétegen belül zárva marad, megakadályozva a belső domain modellek közvetlen kiszivárgását a külvilág felé.
* **Jövőbeli bővíthetőség:** Ha a későbbiekben jogosultságkezelés, gyorsítótárazás (caching) vagy üzleti validáció társulna az olvasásokhoz, a kód azonnal bővíthető a vezérlők módosítása nélkül.

### 3. Termékárak kezelése (Tervezési kompromisszum)
A feladat céljára és méretére való tekintettel a tételsoroknál (`OrderItem`) nem vezettem be ár-pillanatkép mezőt (`UnitPriceAtOrderTime`), a számla összege mindig a `Product.UnitPrice` aktuális értékével kalkulálódik.

> **Megjegyzés:** Nagyvállalati környezetben kritikus követelmény lenne a rendeléskori egységár elmentése a tételsorhoz, megelőzve azt, hogy egy jövőbeni árváltozás utólag módosítsa a korábban lezárt számlák végösszegét.

### 4. Számla entitás kezelése
A számla (`Invoice`) az adatbázisban nem önálló táblaként létezik, hanem a megrendelésből és tételeiből dinamikusan előállított nézetként (`InvoiceDto`), amely a PDF generátor bemeneteként szolgál.

### 5. Domain-Driven Design (DDD) aggregátum elv
Nem készült külön `IOrderItemRepository`. Az `OrderItem` fogalmilag nem létezik függetlenül a megrendeléstől, életciklusa szorosan az `Order`-hez tartozik. Minden módosítás kizárólag a gyökér entitáson (`Order`) keresztül valósul meg.

### 6. Explicit lekérdezések (Lazy Loading elhagyása)
Az összetett lekérdezésekhez az `IOrderRepository` explicit:
```csharp
.Include(o => o.Customer)
.Include(o => o.Items)
    .ThenInclude(i => i.Product)
```
láncolást használ, elkerülve az N+1 lekérdezési anomáliát.

### 7. Repository és Unit of Work minta alkalmazása
Bár az EF Core `DbContext` osztálya maga is megvalósítja a Repository és Unit of Work mintákat, az explicit `IRepository<T>` és `IUnitOfWork` réteg bevezetése a határozott réteghatárok, az egységtesztelhetőség és a technológiai függetlenség bemutatását szolgálja.

### 8. Git verziókezelési megközelítés
Tekintettel arra, hogy a projekt egyszemélyes próbafejlesztés keretében készült, a többágas Git Flow helyett az áttekinthető, lineáris committörténetet követtem a `main` ágon. A mérföldköveket jól elkülönülő lépésekben, konvencionális üzenetekkel (*Conventional Commits*) rögzítettem.

### 9. Adatbázis-motor választása (SQLite)
A specifikáció relációs adatbázist írt elő lokális futtathatósággal. 
A választás tudatosan esett az **SQLite**-ra:
* **Hordozhatóság és Developer Experience:** Nem szükséges különálló adatbázis-szervert (MSSQL, PostgreSQL) telepítenie vagy Docker konténert indítania; a projekt egyetlen `dotnet run` paranccsal azonnal felépül és működik.
* **EF Core absztrakció:** Mivel az adatkapcsolat az Entity Framework Core-on és a repository rétegen keresztül van absztrahálva, az alkalmazás mindössze a connection string és a provider átállításával bármikor átkonfigurálható vállalati szintű SQL motorra.

### 10. Dokumentumformátum és eszközválasztás (QuestPDF)
A feladat szabadon választott formátumot engedélyezett a számlához (TXT, HTML, PDF). A TXT helyett a **QuestPDF** könyvtárral előállított valós PDF formátumot választottam:
* Valósághű számlaképet ad, miközben a teljes elrendezés és dizájn C# kódból, verziókezelhető módon karbantartható.

### 11. Pénzügyi adatok pontossága (`decimal` típus használata)
Az egységárak és a végösszeg tárolására, illetve kalkulálására kizárólag `decimal` (és EF Core oldalon explicit `decimal(18,2)`) típust használtam. Ez kizárja a `float` vagy `double` lebegőpontos reprezentációjából fakadó filléres kerekítési és pontossági hibákat az összegek számításakor.

### 12. Entitás-visszaadás elkerülése minden Service metódusnál (`CreateOrderAsync` javítás)
A fejlesztés során kiderült, hogy a rendelés-létrehozás (`CreateOrderAsync`) eredetileg a nyers `Order` Domain entitást adta vissza a Controllernek. Mivel az EF Core change tracker automatikus navigation property fixup-ot végez (a `Product`, `Order` és `OrderItem` közötti kétirányú hivatkozások miatt), ez körkörös hivatkozási hibát (JSON serialization cycle) okozott szerializáláskor.

**Javítás:** a `CreateOrderAsync` mostantól `OrderDto`-t ad vissza — mentés után a rendelést explicit `Include`-okkal újratölti (`GetByIdWithDetailsAsync`), majd DTO-vá alakítja. Ezzel az összes Service metódus (`GetAll`, `GetById`, `Create`) konzisztensen kizárólag DTO-kon keresztül kommunikál a Controllerekkel — sehol sem szivárog ki nyers Domain entitás az API válaszban.

### 13. Egységes hibaformátum (RFC 7807 ProblemDetails)
A hibaválaszok kezelése a `.NET 8+`-ban bevezetett `IExceptionHandler` interfésszel, egy globális `GlobalExceptionHandler` osztályban központosított. Ez biztosítja, hogy minden hiba (validációs, üzleti logikai, kezeletlen) egységes, RFC 7807 szabványú `ProblemDetails` JSON struktúrában térjen vissza. A hiányzó erőforrásra hivatkozás (pl. nem létező `CustomerId`/`ProductId` egy rendelésnél) egyedi `NotFoundException` típussal jelzett, 
ami HTTP 404-et eredményez — megkülönböztetve a valódi bemeneti hibáktól (`ArgumentException` → 400).

### 14. Bemeneti validáció Data Annotations-szel
A bemeneti (`Create*`) DTO-k mezőin `System.ComponentModel.DataAnnotations` attribútumok (`[Required]`, `[Range]`, `[StringLength]`) biztosítják, hogy a formailag hibás adatok (pl. negatív ár, üres név, érvénytelen azonosító) már a modellkötés szintjén, a Service réteg elérése előtt kiszűrésre kerüljenek. A kimeneti (kizárólag szerver által generált) DTO-kon nincs validáció, mivel ezeket a kliens sosem küldi.