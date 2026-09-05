# Invoicing App

# Fontos döntések fejlesztés során:

Törékeny termékek kezelése (IsFragile mező bevezetése)
A leírás a törékeny termékek speciális jelölését kéri a számlasoron, de az adatmodell csak IsHazardous mezőt definiál — ezért felvettem egy külön IsFragile mezőt, hogy a két fogalmat (veszélyes vs. törékeny) ne keverjem össze.

Termékárak kezelése (Egyszerűsítés / Tervezési kompromisszum)
A feladat specifikációjának egyszerűsége és a scope ésszerű keretek között tartása érdekében a tételeknél (OrderItem) nem vezettem be külön historikus árrögzítő mezőt (UnitPriceAtOrderTime), 
a számla végösszegének számítása mindig a Product.UnitPrice aktuális értékével történik.

A számla nem önálló perzisztált adatbázis-entitás, hanem a rendelési adatokból és tételekből dinamikusan előállított üzleti dokumentum.

Git munkafolyamat és verziókezelés
Mivel a feladat egy egyszemélyes próbafejlesztés volt, a komplex feature branching / Git flow helyett a linear commit history megközelítést alkalmaztam a main ágon. 
A fejlesztési lépéseket jól elkülönülő, logikai egységekre bontva, egyértelmű commit üzenetekkel rögzítettem. 
Csapatmunka esetén természetesen a feature branch-eken alapuló PR (Pull Request) és kód review folyamatokat követném.

Nem csinálok külön IOrderItemRepository-t — az OrderItem mindig az Order aggregátumán belül kezelendő (nem áll önmagában értelmesen), ezért ezt az IOrderRepository-n keresztül fogjuk kezelni majd a Service rétegben.

A .Include(...).ThenInclude(...) lánc azért kell, mert alapértelmezetten EF Core nem tölti be automatikusan a navigációs property-ket (nincs lazy loading bekapcsolva, ami jó is, mert explicit és kiszámítható) — ezért kell explicit jelezni, hogy a Customer-t és az Items-en keresztül a Product-okat is hozza be egy lekérdezésben.