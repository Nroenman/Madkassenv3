†*
UC:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Services\UserService.cs
	namespace 	
MadkassenRestAPI
 
. 
Services #
{ 
public 

class 
UserService 
: 
IUserService +
{ 
private		 
readonly		  
ApplicationDbContext		 -
_context		. 6
;		6 7
private

 
readonly

 
IConfiguration

 '
_configuration

( 6
;

6 7
public 
UserService 
(  
ApplicationDbContext /
context0 7
,7 8
IConfiguration9 G
configurationH U
)U V
{ 	
_context 
= 
context 
; 
_configuration 
= 
configuration *
;* +
} 	
public 
User 
Authenticate  
(  !
string! '
email( -
,- .
string/ 5
password6 >
)> ?
{ 	
var 
user 
= 
_context 
.  
Users  %
.% &
FirstOrDefault& 4
(4 5
u5 6
=>7 9
u: ;
.; <
Email< A
==B D
emailE J
)J K
;K L
if 
( 
user 
== 
null 
|| 
!  !
BCrypt! '
.' (
Net( +
.+ ,
BCrypt, 2
.2 3
Verify3 9
(9 :
password: B
,B C
userD H
.H I
PasswordHashI U
)U V
)V W
{ 
return 
null 
; 
} 
return 
new 
User 
{ 
UserId 
= 
user 
. 
UserId $
,$ %
UserName 
= 
user 
.  
UserName  (
,( )
Email   
=   
user   
.   
Email   "
,  " #
Roles!! 
=!! 
user!! 
.!! 
Roles!! "
,!!" #
	CreatedAt"" 
="" 
user""  
.""  !
	CreatedAt""! *
,""* +
	UpdatedAt## 
=## 
user##  
.##  !
	UpdatedAt##! *
}$$ 
;$$ 
}%% 	
public'' 
User'' 
GetUserFromJwtToken'' '
(''' (
string''( .
token''/ 4
)''4 5
{(( 	
var)) 

jwtHandler)) 
=)) 
new))  #
JwtSecurityTokenHandler))! 8
())8 9
)))9 :
;)): ;
var** 
jwtToken** 
=** 

jwtHandler** %
.**% &
	ReadToken**& /
(**/ 0
token**0 5
)**5 6
as**7 9
JwtSecurityToken**: J
;**J K
if,, 
(,, 
jwtToken,, 
==,, 
null,,  
),,  !
throw-- 
new-- '
UnauthorizedAccessException-- 5
(--5 6
$str--6 E
)--E F
;--F G
var// 
userName// 
=// 
jwtToken// #
?//# $
.//$ %
Claims//% +
?//+ ,
.//, -
FirstOrDefault//- ;
(//; <
c//< =
=>//> @
c//A B
.//B C
Type//C G
==//H J
$str//K P
)//P Q
?//Q R
.//R S
Value//S X
;//X Y
if11 
(11 
string11 
.11 
IsNullOrEmpty11 $
(11$ %
userName11% -
)11- .
)11. /
throw22 
new22 '
UnauthorizedAccessException22 5
(225 6
$str226 E
)22E F
;22F G
var44 
user44 
=44 
_context44 
.44  
Users44  %
.44% &
FirstOrDefault44& 4
(444 5
u445 6
=>447 9
u44: ;
.44; <
UserName44< D
==44E G
userName44H P
)44P Q
;44Q R
if66 
(66 
user66 
==66 
null66 
)66 
throw77 
new77 '
UnauthorizedAccessException77 5
(775 6
$str776 F
)77F G
;77G H
return99 
new99 
User99 
{:: 
UserId;; 
=;; 
user;; 
.;; 
UserId;; $
,;;$ %
UserName<< 
=<< 
user<< 
.<<  
UserName<<  (
,<<( )
Email== 
=== 
user== 
.== 
Email== "
,==" #
Roles>> 
=>> 
user>> 
.>> 
Roles>> "
,>>" #
	CreatedAt?? 
=?? 
user??  
.??  !
	CreatedAt??! *
,??* +
	UpdatedAt@@ 
=@@ 
user@@  
.@@  !
	UpdatedAt@@! *
}AA 
;AA 
}BB 	
}CC 
}DD ª:
fC:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Services\ReservationExpirationService.cs
	namespace 	
MadkassenRestAPI
 
. 
Services #
{ 
public 

class (
ReservationExpirationService -
:. /
BackgroundService0 A
{ 
private 
readonly 
IServiceProvider )
_serviceProvider* :
;: ;
private 
readonly 
ILogger  
<  !(
ReservationExpirationService! =
>= >
_logger? F
;F G
public (
ReservationExpirationService +
(+ ,
IServiceProvider, <
serviceProvider= L
,L M
ILoggerN U
<U V(
ReservationExpirationServiceV r
>r s
loggert z
)z {
{ 	
_serviceProvider 
= 
serviceProvider .
;. /
_logger 
= 
logger 
; 
} 	
	protected 
override 
async  
Task! %
ExecuteAsync& 2
(2 3
CancellationToken3 D
stoppingTokenE R
)R S
{ 	
_logger 
. 
LogInformation "
(" #
$str# J
)J K
;K L
while 
( 
! 
stoppingToken !
.! "#
IsCancellationRequested" 9
)9 :
{ 
try 
{ 
_logger   
.   
LogInformation   *
(  * +
$str  + O
)  O P
;  P Q
await!! "
PerformExpirationLogic!! 0
(!!0 1
stoppingToken!!1 >
)!!> ?
;!!? @
await$$ 
Task$$ 
.$$ 
Delay$$ $
($$$ %
TimeSpan$$% -
.$$- .
FromMinutes$$. 9
($$9 :
$num$$: ;
)$$; <
,$$< =
stoppingToken$$> K
)$$K L
;$$L M
}%% 
catch&& 
(&& !
TaskCanceledException&& ,
)&&, -
{'' 
_logger(( 
.(( 
LogInformation(( *
(((* +
$str((+ \
)((\ ]
;((] ^
break)) 
;)) 
}** 
catch++ 
(++ 
	Exception++  
ex++! #
)++# $
{,, 
_logger-- 
.-- 
LogError-- $
(--$ %
ex--% '
,--' (
$str--) a
)--a b
;--b c
}.. 
}// 
_logger11 
.11 
LogInformation11 "
(11" #
$str11# J
)11J K
;11K L
}22 	
private44 
async44 
Task44 "
PerformExpirationLogic44 1
(441 2
CancellationToken442 C
stoppingToken44D Q
)44Q R
{55 	
using66 
(66 
var66 
scope66 
=66 
_serviceProvider66 /
.66/ 0
CreateScope660 ;
(66; <
)66< =
)66= >
{77 
var88 
context88 
=88 
scope88 #
.88# $
ServiceProvider88$ 3
.883 4
GetRequiredService884 F
<88F G 
ApplicationDbContext88G [
>88[ \
(88\ ]
)88] ^
;88^ _
var99 
now99 
=99 
DateTime99 "
.99" #
UtcNow99# )
;99) *
using;; 
(;; 
var;; 
transaction;; &
=;;' (
await;;) .
context;;/ 6
.;;6 7
Database;;7 ?
.;;? @!
BeginTransactionAsync;;@ U
(;;U V
stoppingToken;;V c
);;c d
);;d e
{<< 
try== 
{>> 
var@@ 
expiredCartItems@@ ,
=@@- .
await@@/ 4
context@@5 <
.@@< =
	CartItems@@= F
.AA 
WhereAA "
(AA" #
cartItemAA# +
=>AA, .
cartItemAA/ 7
.AA7 8
ExpirationTimeAA8 F
<=AAG I
nowAAJ M
)AAM N
.BB 
ToListAsyncBB (
(BB( )
stoppingTokenBB) 6
)BB6 7
;BB7 8
ifDD 
(DD 
expiredCartItemsDD ,
.DD, -
AnyDD- 0
(DD0 1
)DD1 2
)DD2 3
{EE 
_loggerFF #
.FF# $
LogInformationFF$ 2
(FF2 3
$"FF3 5
$strFF5 ;
{FF; <
expiredCartItemsFF< L
.FFL M
CountFFM R
}FFR S
$strFFS g
"FFg h
)FFh i
;FFi j
foreachHH #
(HH$ %
varHH% (
itemHH) -
inHH. 0
expiredCartItemsHH1 A
)HHA B
{II 
varKK  #
productKK$ +
=KK, -
awaitKK. 3
contextKK4 ;
.KK; <
	ProdukterKK< E
.KKE F
	FindAsyncKKF O
(KKO P
itemKKP T
.KKT U
	ProductIdKKU ^
)KK^ _
;KK_ `
ifLL  "
(LL# $
productLL$ +
!=LL, .
nullLL/ 3
)LL3 4
{MM  !
productNN$ +
.NN+ ,

StockLevelNN, 6
+=NN7 9
itemNN: >
.NN> ?
QuantityNN? G
;NNG H
_loggerOO$ +
.OO+ ,
LogInformationOO, :
(OO: ;
$"OO; =
$strOO= Y
{OOY Z
productOOZ a
.OOa b
	ProductIdOOb k
}OOk l
$str	OOl Å
{
OOÅ Ç
item
OOÇ Ü
.
OOÜ á
Quantity
OOá è
}
OOè ê
"
OOê ë
)
OOë í
;
OOí ì
}PP  !
contextSS  '
.SS' (
	CartItemsSS( 1
.SS1 2
RemoveSS2 8
(SS8 9
itemSS9 =
)SS= >
;SS> ?
_loggerTT  '
.TT' (
LogInformationTT( 6
(TT6 7
$"TT7 9
$strTT9 [
{TT[ \
itemTT\ `
.TT` a
	ProductIdTTa j
}TTj k
$strTTk l
"TTl m
)TTm n
;TTn o
}UU 
awaitXX !
contextXX" )
.XX) *
SaveChangesAsyncXX* :
(XX: ;
stoppingTokenXX; H
)XXH I
;XXI J
await[[ !
transaction[[" -
.[[- .
CommitAsync[[. 9
([[9 :
stoppingToken[[: G
)[[G H
;[[H I
_logger\\ #
.\\# $
LogInformation\\$ 2
(\\2 3
$str\\3 u
)\\u v
;\\v w
}]] 
}^^ 
catch__ 
(__ 
	Exception__ $
ex__% '
)__' (
{`` 
_loggeraa 
.aa  
LogErroraa  (
(aa( )
exaa) +
,aa+ ,
$straa- Y
)aaY Z
;aaZ [
awaitdd 
transactiondd )
.dd) *
RollbackAsyncdd* 7
(dd7 8
stoppingTokendd8 E
)ddE F
;ddF G
_loggeree 
.ee  
LogInformationee  .
(ee. /
$stree/ Y
)eeY Z
;eeZ [
}ff 
}gg 
}hh 
}ii 	
}jj 
}kk ¥,
XC:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Services\ProductService.cs
	namespace 	
MadkassenRestAPI
 
. 
Services #
;# $
public 
class 
ProductService 
(  
ApplicationDbContext 0
context1 8
)8 9
{ 
public		 

async		 
Task		 
<		 
List		 
<		 
	Produkter		 $
>		$ %
>		% &
GetAllProductsAsync		' :
(		: ;
)		; <
{

 
return 
await 
context 
. 
	Produkter &
.& '
ToListAsync' 2
(2 3
)3 4
;4 5
} 
public 

async 
Task 
< 
	Produkter 
>  
GetProductByIdAsync! 4
(4 5
int5 8
id9 ;
); <
{ 
var 
product 
= 
await 
context #
.# $
	Produkter$ -
.- .
	FindAsync. 7
(7 8
id8 :
): ;
;; <
if 

( 
product 
== 
null 
) 
{ 	
return 
null 
; 
} 	
return 
product 
; 
} 
public 

async 
Task 
< 
	Produkter 
>  
AddProductAsync! 0
(0 1
	Produkter1 :
product; B
)B C
{ 
if 

( 
product 
== 
null 
) 
{ 	
throw 
new !
ArgumentNullException +
(+ ,
nameof, 2
(2 3
product3 :
): ;
,; <
$str= V
)V W
;W X
} 	
var 
existingProduct 
= 
await #
context$ +
.+ ,
	Produkter, 5
.   
FirstOrDefaultAsync    
(    !
p  ! "
=>  # %
p  & '
.  ' (
ProductName  ( 3
==  4 6
product  7 >
.  > ?
ProductName  ? J
)  J K
;  K L
if!! 

(!! 
existingProduct!! 
!=!! 
null!! #
)!!# $
{"" 	
throw## 
new## %
InvalidOperationException## /
(##/ 0
$"##0 2
$str##2 D
{##D E
product##E L
.##L M
ProductName##M X
}##X Y
$str##Y i
"##i j
)##j k
;##k l
}$$ 	
context&& 
.&& 
	Produkter&& 
.&& 
Add&& 
(&& 
product&& %
)&&% &
;&&& '
await'' 
context'' 
.'' 
SaveChangesAsync'' &
(''& '
)''' (
;''( )
return(( 
product(( 
;(( 
})) 
public++ 

async++ 
Task++ 
<++ 
	Produkter++ 
>++  #
UpdateProductStockAsync++! 8
(++8 9
int++9 <
id++= ?
,++? @
int++A D
quantity++E M
)++M N
{,, 
var-- 
product-- 
=-- 
await-- 
context-- #
.--# $
	Produkter--$ -
.--- .
	FindAsync--. 7
(--7 8
id--8 :
)--: ;
;--; <
if.. 

(.. 
product.. 
==.. 
null.. 
).. 
{// 	
return00 
null00 
;00 
}11 	
if33 

(33 
quantity33 
<33 
$num33 
&&33 
product33 #
.33# $

StockLevel33$ .
<33/ 0
Math331 5
.335 6
Abs336 9
(339 :
quantity33: B
)33B C
)33C D
{44 	
return55 
null55 
;55 
}66 	
product88 
.88 

StockLevel88 
+=88 
quantity88 &
;88& '
await:: 
context:: 
.:: 
SaveChangesAsync:: &
(::& '
)::' (
;::( )
return;; 
product;; 
;;; 
}<< 
public>> 

async>> 
Task>> 
<>> 
List>> 
<>> 
	Produkter>> $
>>>$ %
>>>% &&
GetProductsByCategoryAsync>>' A
(>>A B
int>>B E

categoryId>>F P
)>>P Q
{?? 
var@@ 
products@@ 
=@@ 
await@@ 
context@@ $
.@@$ %
	Produkter@@% .
.AA 
WhereAA 
(AA 
pAA 
=>AA 
pAA 
.AA 

CategoryIdAA $
==AA% '

categoryIdAA( 2
)AA2 3
.BB 
OrderByBB 
(BB 
pBB 
=>BB 
pBB 
.BB 
PriceBB !
)BB! "
.CC 
ToListAsyncCC 
(CC 
)CC 
;CC 
returnEE 
productsEE 
??EE 
newEE 
ListEE #
<EE# $
	ProdukterEE$ -
>EE- .
(EE. /
)EE/ 0
;EE0 1
}FF 
}HH ìG
VC:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Services\OrderService.cs
	namespace 	
MadkassenRestAPI
 
. 
Services #
;# $
public 
class 
OrderService 
(  
ApplicationDbContext .
context/ 6
)6 7
{ 
public		 

async		 
Task		 
<		 
int		 
>		 
CreateOrderAsync		 +
(		+ ,
int		, /
userId		0 6
)		6 7
{

 
var 
	cartItems 
= 
await 
context %
.% &
	CartItems& /
. 
Where 
( 
ci 
=> 
ci 
. 
UserId "
==# %
userId& ,
), -
. 
Include 
( 
ci 
=> 
ci 
. 
	Produkter '
)' (
. 
ToListAsync 
( 
) 
; 
if 

( 
	cartItems 
. 
Count 
== 
$num  
)  !
{ 	
throw 
new %
InvalidOperationException /
(/ 0
$str0 M
)M N
;N O
} 	
decimal 
totalAmount 
= 
	cartItems '
.' (
Sum( +
(+ ,
ci, .
=>/ 1
ci2 4
.4 5
Quantity5 =
*> ?
ci@ B
.B C
	ProdukterC L
.L M
PriceM R
)R S
;S T
var 
newOrder 
= 
new 
Order  
{ 	
UserId 
= 
userId 
, 
	OrderDate 
= 
DateTime  
.  !
UtcNow! '
,' (
OrderStatus 
= 
$str #
,# $
TotalAmount 
= 
totalAmount %
}   	
;  	 

context"" 
."" 
Orders"" 
."" 
Add"" 
("" 
newOrder"" #
)""# $
;""$ %
await## 
context## 
.## 
SaveChangesAsync## &
(##& '
)##' (
;##( )
var&& 
orderId&& 
=&& 
newOrder&& 
.&& 
OrderId&& &
;&&& '
var)) 

orderItems)) 
=)) 
	cartItems)) "
.))" #
Select))# )
())) *
ci))* ,
=>))- /
new))0 3
	OrderItem))4 =
{** 	
OrderId++ 
=++ 
orderId++ 
,++ 
	ProductId,, 
=,, 
ci,, 
.,, 
	ProductId,, $
,,,$ %
Quantity-- 
=-- 
ci-- 
.-- 
Quantity-- "
,--" #
Price.. 
=.. 
ci.. 
... 
	Produkter..  
...  !
Price..! &
,..& '
ProductName// 
=// 
ci// 
.// 
	Produkter// &
.//& '
ProductName//' 2
}00 	
)00	 

.00
 
ToList00 
(00 
)00 
;00 
context22 
.22 

OrderItems22 
.22 
AddRange22 #
(22# $

orderItems22$ .
)22. /
;22/ 0
await33 
context33 
.33 
SaveChangesAsync33 &
(33& '
)33' (
;33( )
context66 
.66 
	CartItems66 
.66 
RemoveRange66 %
(66% &
	cartItems66& /
)66/ 0
;660 1
await77 
context77 
.77 
SaveChangesAsync77 &
(77& '
)77' (
;77( )
return:: 
orderId:: 
;:: 
};; 
public== 

async== 
Task== 
<== 
List== 
<== 
ProductSummary== )
>==) *
>==* +%
GetTopProductsByUserAsync==, E
(==E F
int==F I
userId==J P
,==P Q
int==R U
days==V Z
)==Z [
{>> 
var?? 
fromDate?? 
=?? 
DateTime?? 
.??  
UtcNow??  &
.??& '
AddDays??' .
(??. /
-??/ 0
days??0 4
)??4 5
;??5 6
returnAA 
awaitAA 
contextAA 
.AA 

OrderItemsAA '
.BB 
WhereBB 
(BB 
oiBB 
=>BB 
oiBB 
.BB 
OrderBB !
.BB! "
	OrderDateBB" +
>=BB, .
fromDateBB/ 7
&&BB8 :
oiBB; =
.BB= >
OrderBB> C
.BBC D
UserIdBBD J
==BBK M
userIdBBN T
)BBT U
.CC 
GroupByCC 
(CC 
oiCC 
=>CC 
newCC 
{CC  
oiCC! #
.CC# $
	ProductIdCC$ -
,CC- .
oiCC/ 1
.CC1 2
ProductNameCC2 =
,CC= >
oiCC? A
.CCA B
	ProdukterCCB K
.CCK L
ImageUrlCCL T
}CCT U
)CCU V
.DD 
SelectDD 
(DD 
groupDD 
=>DD 
newDD  
ProductSummaryDD! /
{EE 
	ProductIdFF 
=FF 
groupFF !
.FF! "
KeyFF" %
.FF% &
	ProductIdFF& /
,FF/ 0
ProductNameGG 
=GG 
groupGG #
.GG# $
KeyGG$ '
.GG' (
ProductNameGG( 3
,GG3 4
ImageUrlHH 
=HH 
groupHH  
.HH  !
KeyHH! $
.HH$ %
ImageUrlHH% -
,HH- .
TotalQuantityII 
=II 
groupII  %
.II% &
SumII& )
(II) *
gII* +
=>II, .
gII/ 0
.II0 1
QuantityII1 9
)II9 :
}JJ 
)JJ 
.KK 
OrderByDescendingKK 
(KK 
psKK !
=>KK" $
psKK% '
.KK' (
TotalQuantityKK( 5
)KK5 6
.LL 
TakeLL 
(LL 
$numLL 
)LL 
.MM 
ToListAsyncMM 
(MM 
)MM 
;MM 
}NN 
publicPP 

asyncPP 
TaskPP 
<PP 
ListPP 
<PP 
ProductSummaryPP )
>PP) *
>PP* +&
GetTopProductsOverallAsyncPP, F
(PPF G
intPPG J
daysPPK O
)PPO P
{QQ 
varRR 
fromDateRR 
=RR 
DateTimeRR 
.RR  
UtcNowRR  &
.RR& '
AddDaysRR' .
(RR. /
-RR/ 0
daysRR0 4
)RR4 5
;RR5 6
returnTT 
awaitTT 
contextTT 
.TT 

OrderItemsTT '
.UU 
WhereUU 
(UU 
oiUU 
=>UU 
oiUU 
.UU 
OrderUU !
.UU! "
	OrderDateUU" +
>=UU, .
fromDateUU/ 7
)UU7 8
.VV 
GroupByVV 
(VV 
oiVV 
=>VV 
newVV 
{VV  
oiVV! #
.VV# $
	ProductIdVV$ -
,VV- .
oiVV/ 1
.VV1 2
ProductNameVV2 =
,VV= >
oiVV? A
.VVA B
	ProdukterVVB K
.VVK L
ImageUrlVVL T
}VVU V
)VVV W
.WW 
SelectWW 
(WW 
groupWW 
=>WW 
newWW  
ProductSummaryWW! /
{XX 
	ProductIdYY 
=YY 
groupYY !
.YY! "
KeyYY" %
.YY% &
	ProductIdYY& /
,YY/ 0
ProductNameZZ 
=ZZ 
groupZZ #
.ZZ# $
KeyZZ$ '
.ZZ' (
ProductNameZZ( 3
,ZZ3 4
ImageUrl[[ 
=[[ 
group[[  
.[[  !
Key[[! $
.[[$ %
ImageUrl[[% -
,[[- .
TotalQuantity\\ 
=\\ 
group\\  %
.\\% &
Sum\\& )
(\\) *
g\\* +
=>\\, .
g\\/ 0
.\\0 1
Quantity\\1 9
)\\9 :
}]] 
)]] 
.^^ 
OrderByDescending^^ 
(^^ 
ps^^ !
=>^^" $
ps^^% '
.^^' (
TotalQuantity^^( 5
)^^5 6
.__ 
Take__ 
(__ 
$num__ 
)__ 
.`` 
ToListAsync`` 
(`` 
)`` 
;`` 
}aa 
}bb Ì
VC:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Services\IUserService.cs
	namespace 	
MadkassenRestAPI
 
. 
Services #
{ 
public 

	interface 
IUserService !
{ 
User 
Authenticate 
( 
string  
email! &
,& '
string( .
password/ 7
)7 8
;8 9
User 
GetUserFromJwtToken  
(  !
string! '
token( -
)- .
;. /
}		 
}

 ©T
UC:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Services\CartService.cs
public 
class 
CartService 
{		 
private

 
readonly

  
ApplicationDbContext

 )
_context

* 2
;

2 3
public 

CartService 
(  
ApplicationDbContext +
context, 3
)3 4
{ 
_context 
= 
context 
; 
} 
public 

async 
Task 
AddToCartAsync $
($ %
int% (
	productId) 2
,2 3
int4 7
?7 8
userId9 ?
,? @
intA D
quantityE M
)M N
{ 
var 
existingCartItem 
= 
await $
_context% -
.- .
	CartItems. 7
. 
FirstOrDefaultAsync  
(  !
ci! #
=>$ &
ci' )
.) *
	ProductId* 3
==4 6
	productId7 @
&&A C
ciD F
.F G
UserIdG M
==N P
userIdQ W
)W X
;X Y
var 
product 
= 
await 
_context $
.$ %
	Produkter% .
.. /
	FindAsync/ 8
(8 9
	productId9 B
)B C
;C D
if 

( 
product 
== 
null 
) 
{ 	
throw 
new %
InvalidOperationException /
(/ 0
$str0 D
)D E
;E F
} 	
if 

( 
product 
. 

StockLevel 
<  
quantity! )
)) *
{ 	
throw 
new %
InvalidOperationException /
(/ 0
$str0 M
)M N
;N O
} 	
using!! 
(!! 
var!! 
transaction!! 
=!!  
await!!! &
_context!!' /
.!!/ 0
Database!!0 8
.!!8 9!
BeginTransactionAsync!!9 N
(!!N O
)!!O P
)!!P Q
{"" 	
try## 
{$$ 
if%% 
(%% 
existingCartItem%% $
!=%%% '
null%%( ,
)%%, -
{&& 
existingCartItem(( $
.(($ %
Quantity((% -
+=((. 0
quantity((1 9
;((9 :
_context)) 
.)) 
	CartItems)) &
.))& '
Update))' -
())- .
existingCartItem)). >
)))> ?
;))? @
}** 
else++ 
{,, 
var.. 
cartItem..  
=..! "
new..# &
CartItem..' /
{// 
	ProductId00 !
=00" #
	productId00$ -
,00- .
UserId11 
=11  
userId11! '
,11' (
Quantity22  
=22! "
quantity22# +
,22+ ,
AddedAt33 
=33  !
DateTime33" *
.33* +
UtcNow33+ 1
,331 2
ExpirationTime44 &
=44' (
DateTime44) 1
.441 2
UtcNow442 8
.448 9

AddMinutes449 C
(44C D
$num44D F
)44F G
}55 
;55 
_context77 
.77 
	CartItems77 &
.77& '
Add77' *
(77* +
cartItem77+ 3
)773 4
;774 5
}88 
product:: 
.:: 

StockLevel:: "
-=::# %
quantity::& .
;::. /
await<< 
_context<< 
.<< 
SaveChangesAsync<< /
(<</ 0
)<<0 1
;<<1 2
await== 
transaction== !
.==! "
CommitAsync==" -
(==- .
)==. /
;==/ 0
}>> 
catch?? 
(?? 
	Exception?? 
)?? 
{@@ 
awaitAA 
transactionAA !
.AA! "
RollbackAsyncAA" /
(AA/ 0
)AA0 1
;AA1 2
throwBB 
;BB 
}CC 
}DD 	
}EE 
publicGG 

asyncGG 
TaskGG 
UpdateCartItemAsyncGG )
(GG) *
intGG* -
	productIdGG. 7
,GG7 8
intGG9 <
?GG< =
userIdGG> D
,GGD E
intGGF I
newQuantityGGJ U
)GGU V
{HH 
varII 
cartItemII 
=II 
awaitII 
_contextII %
.II% &
	CartItemsII& /
.JJ 
FirstOrDefaultAsyncJJ  
(JJ  !
ciJJ! #
=>JJ$ &
ciJJ' )
.JJ) *
	ProductIdJJ* 3
==JJ4 6
	productIdJJ7 @
&&JJA C
ciJJD F
.JJF G
UserIdJJG M
==JJN P
userIdJJQ W
)JJW X
;JJX Y
ifLL 

(LL 
cartItemLL 
==LL 
nullLL 
)LL 
{MM 	
throwNN 
newNN %
InvalidOperationExceptionNN /
(NN/ 0
$strNN0 F
)NNF G
;NNG H
}OO 	
varQQ 
productQQ 
=QQ 
awaitQQ 
_contextQQ $
.QQ$ %
	ProdukterQQ% .
.QQ. /
	FindAsyncQQ/ 8
(QQ8 9
	productIdQQ9 B
)QQB C
;QQC D
ifSS 

(SS 
productSS 
==SS 
nullSS 
)SS 
{TT 	
throwUU 
newUU %
InvalidOperationExceptionUU /
(UU/ 0
$strUU0 D
)UUD E
;UUE F
}VV 	
intYY 
stockAdjustmentYY 
=YY 
cartItemYY &
.YY& '
QuantityYY' /
-YY0 1
newQuantityYY2 =
;YY= >
if[[ 

([[ 
product[[ 
.[[ 

StockLevel[[ 
+[[  
stockAdjustment[[! 0
<[[1 2
$num[[3 4
)[[4 5
{\\ 	
throw]] 
new]] %
InvalidOperationException]] /
(]]/ 0
$str]]0 M
)]]M N
;]]N O
}^^ 	
productaa 
.aa 

StockLevelaa 
+=aa 
stockAdjustmentaa -
;aa- .
cartItembb 
.bb 
Quantitybb 
=bb 
newQuantitybb '
;bb' (
_contextdd 
.dd 
	CartItemsdd 
.dd 
Updatedd !
(dd! "
cartItemdd" *
)dd* +
;dd+ ,
awaitee 
_contextee 
.ee 
SaveChangesAsyncee '
(ee' (
)ee( )
;ee) *
}ff 
publichh 

asynchh 
Taskhh 
RemoveCartItemAsynchh )
(hh) *
inthh* -
	productIdhh. 7
,hh7 8
inthh9 <
?hh< =
userIdhh> D
)hhD E
{ii 
varjj 
cartItemjj 
=jj 
awaitjj 
_contextjj %
.jj% &
	CartItemsjj& /
.kk 
FirstOrDefaultAsynckk  
(kk  !
cikk! #
=>kk$ &
cikk' )
.kk) *
	ProductIdkk* 3
==kk4 6
	productIdkk7 @
&&kkA C
(ll 
cill 
.ll 
UserIdll 
==ll 
userIdll $
||ll% '
(ll( )
cill) +
.ll+ ,
UserIdll, 2
==ll3 5
nullll6 :
&&ll; =
userIdll> D
==llE G
nullllH L
)llL M
)llM N
)llN O
;llO P
ifnn 

(nn 
cartItemnn 
==nn 
nullnn 
)nn 
{oo 	
throwpp 
newpp %
InvalidOperationExceptionpp /
(pp/ 0
$strpp0 F
)ppF G
;ppG H
}qq 	
varss 
productss 
=ss 
awaitss 
_contextss $
.ss$ %
	Produkterss% .
.ss. /
	FindAsyncss/ 8
(ss8 9
	productIdss9 B
)ssB C
;ssC D
ifuu 

(uu 
productuu 
!=uu 
nulluu 
)uu 
{vv 	
productww 
.ww 

StockLevelww 
+=ww !
cartItemww" *
.ww* +
Quantityww+ 3
;ww3 4
}xx 	
_contextzz 
.zz 
	CartItemszz 
.zz 
Removezz !
(zz! "
cartItemzz" *
)zz* +
;zz+ ,
await{{ 
_context{{ 
.{{ 
SaveChangesAsync{{ '
({{' (
){{( )
;{{) *
}|| 
public~~ 

async~~ 
Task~~ 
<~~ 
List~~ 
<~~ 
CartItemDto~~ &
>~~& '
>~~' (%
GetCartItemsByUserIdAsync~~) B
(~~B C
int~~C F
userId~~G M
)~~M N
{ 
return
ÄÄ 
await
ÄÄ 
_context
ÄÄ 
.
ÄÄ 
	CartItems
ÄÄ '
.
ÅÅ 
Where
ÅÅ 
(
ÅÅ 
ci
ÅÅ 
=>
ÅÅ 
ci
ÅÅ 
.
ÅÅ 
UserId
ÅÅ "
==
ÅÅ# %
userId
ÅÅ& ,
)
ÅÅ, -
.
ÇÇ 
Include
ÇÇ 
(
ÇÇ 
ci
ÇÇ 
=>
ÇÇ 
ci
ÇÇ 
.
ÇÇ 
	Produkter
ÇÇ '
)
ÇÇ' (
.
ÉÉ 
Select
ÉÉ 
(
ÉÉ 
ci
ÉÉ 
=>
ÉÉ 
new
ÉÉ 
CartItemDto
ÉÉ )
{
ÑÑ 
	ProductId
ÖÖ 
=
ÖÖ 
ci
ÖÖ 
.
ÖÖ 
	ProductId
ÖÖ (
,
ÖÖ( )
UserId
ÜÜ 
=
ÜÜ 
ci
ÜÜ 
.
ÜÜ 
UserId
ÜÜ "
,
ÜÜ" #
Quantity
áá 
=
áá 
ci
áá 
.
áá 
Quantity
áá &
,
áá& '
ProductName
àà 
=
àà 
ci
àà  
.
àà  !
	Produkter
àà! *
.
àà* +
ProductName
àà+ 6
,
àà6 7
Price
ââ 
=
ââ 
(
ââ 
decimal
ââ  
)
ââ  !
ci
ââ! #
.
ââ# $
	Produkter
ââ$ -
.
ââ- .
Price
ââ. 3
,
ââ3 4
}
ää 
)
ää 
.
ãã 
ToListAsync
ãã 
(
ãã 
)
ãã 
;
ãã 
}
åå 
}çç Ï
\C:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Repositories\UserRepository.cs
	namespace 	
MadkassenRestAPI
 
. 
Repositories '
{ 
public 

class 
UserRepository 
(   
ApplicationDbContext  4
context5 <
)< =
:> ?
IUserRepository@ O
{ 
public

 
User

 
?

 

GetByEmail

 
(

  
string

  &
email

' ,
)

, -
{ 	
var 
dbUser 
= 
context  
.  !
Users! &
.& '
FirstOrDefault' 5
(5 6
u6 7
=>8 :
u; <
.< =
Email= B
==C E
emailF K
)K L
;L M
if 
( 
dbUser 
== 
null 
) 
return  &
null' +
;+ ,
return 
new 
User 
{ 
Email 
= 
dbUser 
. 
Email $
,$ %
PasswordHash 
= 
dbUser %
.% &
PasswordHash& 2
,2 3
UserName 
= 
dbUser !
.! "
UserName" *
,* +
Roles 
= 
dbUser 
. 
Roles $
,$ %
} 
; 
} 	
public 
IEnumerable 
< 
User 
>  
GetAll! '
(' (
)( )
{ 	
throw 
new #
NotImplementedException -
(- .
). /
;/ 0
} 	
} 
} “
]C:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Repositories\IUserRepository.cs
	namespace 	
MadkassenRestAPI
 
. 
Repositories '
{ 
public 

	interface 
IUserRepository $
{ 
User 
? 

GetByEmail 
( 
string 
email  %
)% &
;& '
IEnumerable 
< 
User 
> 
GetAll  
(  !
)! "
;" #
}		 
}

 ∂B
HC:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Program.cs
var

 
builder

 
=

 
WebApplication

 
.

 
CreateBuilder

 *
(

* +
args

+ /
)

/ 0
;

0 1
builder 
. 
Services 
. 
AddControllers 
(  
)  !
;! "
builder 
. 
Services 
. #
AddEndpointsApiExplorer (
(( )
)) *
;* +
builder 
. 
Services 
. "
AddHttpContextAccessor '
(' (
)( )
;) *
builder 
. 
Services 
. 
	AddScoped 
< 
IUserRepository *
,* +
UserRepository, :
>: ;
(; <
)< =
;= >
builder 
. 
Services 
. 
	AddScoped 
< 
CartService &
>& '
(' (
)( )
;) *
builder 
. 
Services 
. 
	AddScoped 
< 
OrderService '
>' (
(( )
)) *
;* +
builder 
. 
Services 
. 
	AddScoped 
< 
ProductService )
>) *
(* +
)+ ,
;, -
builder 
. 
Services 
. 
	AddScoped 
< (
ReservationExpirationService 7
>7 8
(8 9
)9 :
;: ;
builder 
. 
Services 
. 
	AddScoped 
< 
UserService &
>& '
(' (
)( )
;) *
builder 
. 
Services 
. 
	AddScoped 
< 
IUserService '
,' (
UserService) 4
>4 5
(5 6
)6 7
;7 8
builder 
. 
Services 
. 
AddDbContext 
<  
ApplicationDbContext 2
>2 3
(3 4
options4 ;
=>< >
options 
. 
UseSqlServer 
( 
builder  
.  !
Configuration! .
.. /
GetConnectionString/ B
(B C
$strC V
)V W
)W X
)X Y
;Y Z
builder 
. 
Services 
. 
AddHostedService !
<! "(
ReservationExpirationService" >
>> ?
(? @
)@ A
;A B
builder!! 
.!! 
Services!! 
.!! 
AddCors!! 
(!! 
options!!  
=>!!! #
{"" 
options## 
.## 
	AddPolicy## 
(## 
$str##  
,##  !
policy##" (
=>##) +
{$$ 
policy%% 
.%% 
AllowAnyOrigin%% 
(%% 
)%% 
.&& 
AllowAnyMethod&& 
(&& 
)&& 
.'' 
AllowAnyHeader'' 
('' 
)'' 
;''  
}(( 
)(( 
;(( 
})) 
))) 
;)) 
builder,, 
.,, 
Services,, 
.,, 
AddSwaggerGen,, 
(,, 
options,, &
=>,,' )
{-- 
options.. 
... !
AddSecurityDefinition.. !
(..! "
$str.." *
,..* +
new.., /!
OpenApiSecurityScheme..0 E
{// 
In00 

=00 
ParameterLocation00 
.00 
Header00 %
,00% &
Name11 
=11 
$str11 
,11 
Type22 
=22 
SecuritySchemeType22 !
.22! "
Http22" &
,22& '
Scheme33 
=33 
$str33 
,33 
BearerFormat44 
=44 
$str44 
,44 
Description55 
=55 
$str55 =
}66 
)66 
;66 
options88 
.88 "
AddSecurityRequirement88 "
(88" #
new88# &&
OpenApiSecurityRequirement88' A
{99 
{:: 	
new;; !
OpenApiSecurityScheme;; %
{<< 
	Reference== 
=== 
new== 
OpenApiReference==  0
{>> 
Type?? 
=?? 
ReferenceType?? (
.??( )
SecurityScheme??) 7
,??7 8
Id@@ 
=@@ 
$str@@ !
}AA 
}BB 
,BB 
ArrayCC 
.CC 
EmptyCC 
<CC 
stringCC 
>CC 
(CC  
)CC  !
}DD 	
}EE 
)EE 
;EE 
optionsGG 
.GG 
OperationFilterGG 
<GG /
#SecurityRequirementsOperationFilterGG ?
>GG? @
(GG@ A
)GGA B
;GGB C
}HH 
)HH 
;HH 
builderKK 
.KK 
ServicesKK 
.KK 
AddAuthenticationKK "
(KK" #
$strKK# +
)KK+ ,
.LL 
AddJwtBearerLL 
(LL 
optionsLL 
=>LL 
{MM 
optionsNN 
.NN %
TokenValidationParametersNN )
=NN* +
newNN, /%
TokenValidationParametersNN0 I
{OO 	$
ValidateIssuerSigningKeyPP $
=PP% &
truePP' +
,PP+ ,
ValidateAudienceQQ 
=QQ 
trueQQ #
,QQ# $
ValidateIssuerRR 
=RR 
trueRR !
,RR! "
IssuerSigningKeySS 
=SS 
newSS " 
SymmetricSecurityKeySS# 7
(SS7 8
EncodingSS8 @
.SS@ A
UTF8SSA E
.SSE F
GetBytesSSF N
(SSN O
builderSSO V
.SSV W
ConfigurationSSW d
[SSd e
$strSSe x
]SSx y
!SSy z
)SSz {
)SS{ |
,SS| }
ValidAudienceTT 
=TT 
builderTT #
.TT# $
ConfigurationTT$ 1
[TT1 2
$strTT2 H
]TTH I
,TTI J
ValidIssuerUU 
=UU 
builderUU !
.UU! "
ConfigurationUU" /
[UU/ 0
$strUU0 D
]UUD E
}VV 	
;VV	 

}WW 
)WW 
;WW 
varYY 
appYY 
=YY 	
builderYY
 
.YY 
BuildYY 
(YY 
)YY 
;YY 
if[[ 
([[ 
app[[ 
.[[ 
Environment[[ 
.[[ 
IsDevelopment[[ !
([[! "
)[[" #
)[[# $
{\\ 
app]] 
.]] 

UseSwagger]] 
(]] 
)]] 
;]] 
app^^ 
.^^ 
UseSwaggerUI^^ 
(^^ 
options^^ 
=>^^ 
{__ 
options`` 
.`` 
SwaggerEndpoint`` 
(``  
$str``  :
,``: ;
$str``< N
)``N O
;``O P
optionsaa 
.aa 
RoutePrefixaa 
=aa 
stringaa $
.aa$ %
Emptyaa% *
;aa* +
}bb 
)bb 
;bb 
}cc 
appee 
.ee 
UseCorsee 
(ee 
$stree 
)ee 
;ee 
appff 
.ff 
UseAuthenticationff 
(ff 
)ff 
;ff 
appgg 
.gg 
UseAuthorizationgg 
(gg 
)gg 
;gg 
appjj 
.jj 
UseAuthenticationjj 
(jj 
)jj 
;jj 
appkk 
.kk 
UseAuthorizationkk 
(kk 
)kk 
;kk 
appmm 
.mm 
MapControllersmm 
(mm 
)mm 
;mm 
appoo 
.oo 
Runoo 
(oo 
)oo 	
;oo	 
»
LC:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Models\User.cs
	namespace 	
MadkassenRestAPI
 
. 
Models !
{ 
public 

class 
User 
: 
Users 
{ 
} 
public

 

class

 
	AuthInput

 
{ 
public 
string 
Email 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Password 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} ¿™
\C:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Models\ApplicationDbContext.cs
	namespace 	
MadkassenRestAPI
 
. 
Models !
{ 
public 

class  
ApplicationDbContext %
:& '
	DbContext( 1
{ 
public		  
ApplicationDbContext		 #
(		# $
DbContextOptions		$ 4
<		4 5 
ApplicationDbContext		5 I
>		I J
options		K R
)		R S
:		T U
base		V Z
(		Z [
options		[ b
)		b c
{

 	
} 	
public 
DbSet 
< 
	Produkter 
> 
	Produkter  )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
public 
DbSet 
< 
Kategori 
> 
Kategori '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 
DbSet 
< 
Users 
> 
Users !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
DbSet 
< 
CartItem 
> 
	CartItems (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 
DbSet 
< 
Order 
> 
Orders "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
DbSet 
< 
	OrderItem 
> 

OrderItems  *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
	protected 
override 
void 
OnModelCreating  /
(/ 0
ModelBuilder0 <
modelBuilder= I
)I J
{ 	
modelBuilder 
. 
Entity 
<  
Kategori  (
>( )
() *
)* +
. 
HasKey 
( 
k 
=> 
k 
. 

CategoryId )
)) *
;* +
modelBuilder 
. 
Entity 
<  
Kategori  (
>( )
() *
)* +
. 
Property 
( 
k 
=> 
k  
.  !
CategoryName! -
)- .
. 
HasColumnName 
( 
$str -
)- .
. 
HasMaxLength 
( 
$num !
)! "
. 

IsRequired 
( 
) 
; 
modelBuilder   
.   
Entity   
<    
Kategori    (
>  ( )
(  ) *
)  * +
.!! 
Property!! 
(!! 
k!! 
=>!! 
k!!  
.!!  !
Description!!! ,
)!!, -
."" 
HasColumnName"" 
("" 
$str"" ,
)"", -
;""- .
modelBuilder%% 
.%% 
Entity%% 
<%%  
	Produkter%%  )
>%%) *
(%%* +
)%%+ ,
.&& 
HasKey&& 
(&& 
p&& 
=>&& 
p&& 
.&& 
	ProductId&& (
)&&( )
;&&) *
modelBuilder(( 
.(( 
Entity(( 
<((  
	Produkter((  )
>(() *
(((* +
)((+ ,
.)) 
Property)) 
()) 
p)) 
=>)) 
p))  
.))  !
ProductName))! ,
))), -
.** 
HasColumnName** 
(** 
$str** ,
)**, -
.++ 
HasMaxLength++ 
(++ 
$num++ !
)++! "
.,, 

IsRequired,, 
(,, 
),, 
;,, 
modelBuilder.. 
... 
Entity.. 
<..  
	Produkter..  )
>..) *
(..* +
)..+ ,
.// 
Property// 
(// 
p// 
=>// 
p//  
.//  !
Description//! ,
)//, -
.00 
HasColumnName00 
(00 
$str00 ,
)00, -
;00- .
modelBuilder22 
.22 
Entity22 
<22  
	Produkter22  )
>22) *
(22* +
)22+ ,
.33 
Property33 
(33 
p33 
=>33 
p33  
.33  !
AllergyType33! ,
)33, -
.44 
HasConversion44 
(44 
v55 
=>55 
v55 
.55 
HasValue55 #
?55$ %
v55& '
.55' (
Value55( -
.55- .
ToString55. 6
(556 7
)557 8
:559 :
null55; ?
,55? @
v66 
=>66 
string66 
.66  
IsNullOrEmpty66  -
(66- .
v66. /
)66/ 0
?661 2
(663 4
AllergyType664 ?
?66? @
)66@ A
null66A E
:66F G
Enum66H L
.66L M
Parse66M R
<66R S
AllergyType66S ^
>66^ _
(66_ `
v66` a
)66a b
)77 
;77 
modelBuilder99 
.99 
Entity99 
<99  
	Produkter99  )
>99) *
(99* +
)99+ ,
.:: 
Property:: 
(:: 
p:: 
=>:: 
p::  
.::  !
Price::! &
)::& '
.;; 
HasColumnType;; 
(;; 
$str;; .
);;. /
;;;/ 0
modelBuilder== 
.== 
Entity== 
<==  
	Produkter==  )
>==) *
(==* +
)==+ ,
.>> 
Property>> 
(>> 
p>> 
=>>> 
p>>  
.>>  !

StockLevel>>! +
)>>+ ,
.?? 
HasColumnName?? 
(?? 
$str?? +
)??+ ,
;??, -
modelBuilderBB 
.BB 
EntityBB 
<BB  
UsersBB  %
>BB% &
(BB& '
)BB' (
.CC 
HasKeyCC 
(CC 
uCC 
=>CC 
uCC 
.CC 
UserIdCC %
)CC% &
;CC& '
modelBuilderEE 
.EE 
EntityEE 
<EE  
UsersEE  %
>EE% &
(EE& '
)EE' (
.FF 
PropertyFF 
(FF 
uFF 
=>FF 
uFF  
.FF  !
UserNameFF! )
)FF) *
.GG 
HasColumnNameGG 
(GG 
$strGG )
)GG) *
.HH 
HasMaxLengthHH 
(HH 
$numHH !
)HH! "
.II 

IsRequiredII 
(II 
)II 
;II 
modelBuilderKK 
.KK 
EntityKK 
<KK  
UsersKK  %
>KK% &
(KK& '
)KK' (
.LL 
PropertyLL 
(LL 
uLL 
=>LL 
uLL  
.LL  !
EmailLL! &
)LL& '
.MM 
HasColumnNameMM 
(MM 
$strMM &
)MM& '
.NN 
HasMaxLengthNN 
(NN 
$numNN !
)NN! "
.OO 

IsRequiredOO 
(OO 
)OO 
;OO 
modelBuilderQQ 
.QQ 
EntityQQ 
<QQ  
UsersQQ  %
>QQ% &
(QQ& '
)QQ' (
.RR 
PropertyRR 
(RR 
uRR 
=>RR 
uRR  
.RR  !
PasswordHashRR! -
)RR- .
.SS 
HasColumnNameSS 
(SS 
$strSS -
)SS- .
.TT 
HasMaxLengthTT 
(TT 
$numTT !
)TT! "
.UU 

IsRequiredUU 
(UU 
)UU 
;UU 
modelBuilderWW 
.WW 
EntityWW 
<WW  
UsersWW  %
>WW% &
(WW& '
)WW' (
.XX 
PropertyXX 
(XX 
uXX 
=>XX 
uXX  
.XX  !
	CreatedAtXX! *
)XX* +
.YY 
HasColumnNameYY 
(YY 
$strYY *
)YY* +
.ZZ 
HasColumnTypeZZ 
(ZZ 
$strZZ *
)ZZ* +
.[[ 

IsRequired[[ 
([[ 
true[[  
)[[  !
;[[! "
modelBuilder]] 
.]] 
Entity]] 
<]]  
Users]]  %
>]]% &
(]]& '
)]]' (
.^^ 
Property^^ 
(^^ 
u^^ 
=>^^ 
u^^  
.^^  !
	UpdatedAt^^! *
)^^* +
.__ 
HasColumnName__ 
(__ 
$str__ *
)__* +
.`` 
HasColumnType`` 
(`` 
$str`` *
)``* +
.aa 

IsRequiredaa 
(aa 
trueaa  
)aa  !
;aa! "
modelBuildercc 
.cc 
Entitycc 
<cc  
Userscc  %
>cc% &
(cc& '
)cc' (
.dd 
Propertydd 
(dd 
udd 
=>dd 
udd  
.dd  !
Rolesdd! &
)dd& '
.ee 
HasColumnNameee 
(ee 
$stree &
)ee& '
.ff 
HasMaxLengthff 
(ff 
$numff  
)ff  !
.gg 

IsRequiredgg 
(gg 
)gg 
;gg 
modelBuilderjj 
.jj 
Entityjj 
<jj  
CartItemjj  (
>jj( )
(jj) *
)jj* +
.kk 
HasKeykk 
(kk 
ckk 
=>kk 
ckk 
.kk 

CartItemIdkk )
)kk) *
;kk* +
modelBuildermm 
.mm 
Entitymm 
<mm  
CartItemmm  (
>mm( )
(mm) *
)mm* +
.nn 
Propertynn 
(nn 
cnn 
=>nn 
cnn  
.nn  !

CartItemIdnn! +
)nn+ ,
.oo 
HasColumnNameoo 
(oo 
$stroo +
)oo+ ,
;oo, -
modelBuilderqq 
.qq 
Entityqq 
<qq  
CartItemqq  (
>qq( )
(qq) *
)qq* +
.rr 
Propertyrr 
(rr 
crr 
=>rr 
crr  
.rr  !
	ProductIdrr! *
)rr* +
.ss 
HasColumnNamess 
(ss 
$strss *
)ss* +
.tt 

IsRequiredtt 
(tt 
)tt 
;tt 
modelBuildervv 
.vv 
Entityvv 
<vv  
CartItemvv  (
>vv( )
(vv) *
)vv* +
.ww 
Propertyww 
(ww 
cww 
=>ww 
cww  
.ww  !
UserIdww! '
)ww' (
.xx 
HasColumnNamexx 
(xx 
$strxx '
)xx' (
.yy 

IsRequiredyy 
(yy 
)yy 
;yy 
modelBuilder{{ 
.{{ 
Entity{{ 
<{{  
CartItem{{  (
>{{( )
({{) *
){{* +
.|| 
Property|| 
(|| 
c|| 
=>|| 
c||  
.||  !
Quantity||! )
)||) *
.}} 
HasColumnName}} 
(}} 
$str}} )
)}}) *
.~~ 

IsRequired~~ 
(~~ 
)~~ 
;~~ 
modelBuilder
ÄÄ 
.
ÄÄ 
Entity
ÄÄ 
<
ÄÄ  
CartItem
ÄÄ  (
>
ÄÄ( )
(
ÄÄ) *
)
ÄÄ* +
.
ÅÅ 
HasOne
ÅÅ 
(
ÅÅ 
c
ÅÅ 
=>
ÅÅ 
c
ÅÅ 
.
ÅÅ 
	Produkter
ÅÅ (
)
ÅÅ( )
.
ÇÇ 
WithMany
ÇÇ 
(
ÇÇ 
)
ÇÇ 
.
ÉÉ 
HasForeignKey
ÉÉ 
(
ÉÉ 
c
ÉÉ  
=>
ÉÉ! #
c
ÉÉ$ %
.
ÉÉ% &
	ProductId
ÉÉ& /
)
ÉÉ/ 0
;
ÉÉ0 1
modelBuilder
ÖÖ 
.
ÖÖ 
Entity
ÖÖ 
<
ÖÖ  
CartItem
ÖÖ  (
>
ÖÖ( )
(
ÖÖ) *
)
ÖÖ* +
.
ÜÜ 
HasOne
ÜÜ 
(
ÜÜ 
c
ÜÜ 
=>
ÜÜ 
c
ÜÜ 
.
ÜÜ 
Users
ÜÜ $
)
ÜÜ$ %
.
áá 
WithMany
áá 
(
áá 
)
áá 
.
àà 
HasForeignKey
àà 
(
àà 
c
àà  
=>
àà! #
c
àà$ %
.
àà% &
UserId
àà& ,
)
àà, -
;
àà- .
modelBuilder
ãã 
.
ãã 
Entity
ãã 
<
ãã  
Order
ãã  %
>
ãã% &
(
ãã& '
)
ãã' (
.
åå 
HasKey
åå 
(
åå 
o
åå 
=>
åå 
o
åå 
.
åå 
OrderId
åå &
)
åå& '
;
åå' (
modelBuilder
éé 
.
éé 
Entity
éé 
<
éé  
Order
éé  %
>
éé% &
(
éé& '
)
éé' (
.
èè 
Property
èè 
(
èè 
o
èè 
=>
èè 
o
èè  
.
èè  !
OrderId
èè! (
)
èè( )
.
êê 
HasColumnName
êê 
(
êê 
$str
êê (
)
êê( )
.
ëë !
ValueGeneratedOnAdd
ëë $
(
ëë$ %
)
ëë% &
;
ëë& '
modelBuilder
ìì 
.
ìì 
Entity
ìì 
<
ìì  
Order
ìì  %
>
ìì% &
(
ìì& '
)
ìì' (
.
îî 
Property
îî 
(
îî 
o
îî 
=>
îî 
o
îî  
.
îî  !
UserId
îî! '
)
îî' (
.
ïï 
HasColumnName
ïï 
(
ïï 
$str
ïï '
)
ïï' (
.
ññ 

IsRequired
ññ 
(
ññ 
)
ññ 
;
ññ 
modelBuilder
òò 
.
òò 
Entity
òò 
<
òò  
Order
òò  %
>
òò% &
(
òò& '
)
òò' (
.
ôô 
Property
ôô 
(
ôô 
o
ôô 
=>
ôô 
o
ôô  
.
ôô  !
	OrderDate
ôô! *
)
ôô* +
.
öö 
HasColumnName
öö 
(
öö 
$str
öö *
)
öö* +
.
õõ 
HasColumnType
õõ 
(
õõ 
$str
õõ *
)
õõ* +
.
úú  
HasDefaultValueSql
úú #
(
úú# $
$str
úú$ /
)
úú/ 0
.
ùù 

IsRequired
ùù 
(
ùù 
)
ùù 
;
ùù 
modelBuilder
üü 
.
üü 
Entity
üü 
<
üü  
Order
üü  %
>
üü% &
(
üü& '
)
üü' (
.
†† 
Property
†† 
(
†† 
o
†† 
=>
†† 
o
††  
.
††  !
OrderStatus
††! ,
)
††, -
.
°° 
HasColumnName
°° 
(
°° 
$str
°° ,
)
°°, -
.
¢¢ 
HasMaxLength
¢¢ 
(
¢¢ 
$num
¢¢  
)
¢¢  !
.
££ 
HasDefaultValue
££  
(
££  !
$str
££! *
)
££* +
.
§§ 

IsRequired
§§ 
(
§§ 
)
§§ 
;
§§ 
modelBuilder
¶¶ 
.
¶¶ 
Entity
¶¶ 
<
¶¶  
Order
¶¶  %
>
¶¶% &
(
¶¶& '
)
¶¶' (
.
ßß 
Property
ßß 
(
ßß 
o
ßß 
=>
ßß 
o
ßß  
.
ßß  !
TotalAmount
ßß! ,
)
ßß, -
.
®® 
HasColumnName
®® 
(
®® 
$str
®® ,
)
®®, -
.
©© 
HasColumnType
©© 
(
©© 
$str
©© .
)
©©. /
.
™™ 

IsRequired
™™ 
(
™™ 
)
™™ 
;
™™ 
modelBuilder
≠≠ 
.
≠≠ 
Entity
≠≠ 
<
≠≠  
Order
≠≠  %
>
≠≠% &
(
≠≠& '
)
≠≠' (
.
ÆÆ 
HasMany
ÆÆ 
(
ÆÆ 
o
ÆÆ 
=>
ÆÆ 
o
ÆÆ 
.
ÆÆ  

OrderItems
ÆÆ  *
)
ÆÆ* +
.
ØØ 
WithOne
ØØ 
(
ØØ 
oi
ØØ 
=>
ØØ 
oi
ØØ !
.
ØØ! "
Order
ØØ" '
)
ØØ' (
.
∞∞ 
HasForeignKey
∞∞ 
(
∞∞ 
oi
∞∞ !
=>
∞∞" $
oi
∞∞% '
.
∞∞' (
OrderId
∞∞( /
)
∞∞/ 0
;
∞∞0 1
modelBuilder
≥≥ 
.
≥≥ 
Entity
≥≥ 
<
≥≥  
	OrderItem
≥≥  )
>
≥≥) *
(
≥≥* +
)
≥≥+ ,
.
¥¥ 
HasKey
¥¥ 
(
¥¥ 
oi
¥¥ 
=>
¥¥ 
oi
¥¥  
.
¥¥  !
OrderItemId
¥¥! ,
)
¥¥, -
;
¥¥- .
modelBuilder
∂∂ 
.
∂∂ 
Entity
∂∂ 
<
∂∂  
	OrderItem
∂∂  )
>
∂∂) *
(
∂∂* +
)
∂∂+ ,
.
∑∑ 
Property
∑∑ 
(
∑∑ 
oi
∑∑ 
=>
∑∑ 
oi
∑∑  "
.
∑∑" #
OrderItemId
∑∑# .
)
∑∑. /
.
∏∏ 
HasColumnName
∏∏ 
(
∏∏ 
$str
∏∏ ,
)
∏∏, -
.
ππ !
ValueGeneratedOnAdd
ππ $
(
ππ$ %
)
ππ% &
;
ππ& '
modelBuilder
ªª 
.
ªª 
Entity
ªª 
<
ªª  
	OrderItem
ªª  )
>
ªª) *
(
ªª* +
)
ªª+ ,
.
ºº 
Property
ºº 
(
ºº 
oi
ºº 
=>
ºº 
oi
ºº  "
.
ºº" #
OrderId
ºº# *
)
ºº* +
.
ΩΩ 
HasColumnName
ΩΩ 
(
ΩΩ 
$str
ΩΩ (
)
ΩΩ( )
.
ææ 

IsRequired
ææ 
(
ææ 
)
ææ 
;
ææ 
modelBuilder
¿¿ 
.
¿¿ 
Entity
¿¿ 
<
¿¿  
	OrderItem
¿¿  )
>
¿¿) *
(
¿¿* +
)
¿¿+ ,
.
¡¡ 
Property
¡¡ 
(
¡¡ 
oi
¡¡ 
=>
¡¡ 
oi
¡¡  "
.
¡¡" #
	ProductId
¡¡# ,
)
¡¡, -
.
¬¬ 
HasColumnName
¬¬ 
(
¬¬ 
$str
¬¬ *
)
¬¬* +
.
√√ 

IsRequired
√√ 
(
√√ 
)
√√ 
;
√√ 
modelBuilder
≈≈ 
.
≈≈ 
Entity
≈≈ 
<
≈≈  
	OrderItem
≈≈  )
>
≈≈) *
(
≈≈* +
)
≈≈+ ,
.
∆∆ 
Property
∆∆ 
(
∆∆ 
oi
∆∆ 
=>
∆∆ 
oi
∆∆  "
.
∆∆" #
Quantity
∆∆# +
)
∆∆+ ,
.
«« 
HasColumnName
«« 
(
«« 
$str
«« )
)
««) *
.
»» 

IsRequired
»» 
(
»» 
)
»» 
;
»» 
modelBuilder
   
.
   
Entity
   
<
    
	OrderItem
    )
>
  ) *
(
  * +
)
  + ,
.
ÀÀ 
Property
ÀÀ 
(
ÀÀ 
oi
ÀÀ 
=>
ÀÀ 
oi
ÀÀ  "
.
ÀÀ" #
Price
ÀÀ# (
)
ÀÀ( )
.
ÃÃ 
HasColumnName
ÃÃ 
(
ÃÃ 
$str
ÃÃ &
)
ÃÃ& '
.
ÕÕ 
HasColumnType
ÕÕ 
(
ÕÕ 
$str
ÕÕ .
)
ÕÕ. /
.
ŒŒ 

IsRequired
ŒŒ 
(
ŒŒ 
)
ŒŒ 
;
ŒŒ 
modelBuilder
–– 
.
–– 
Entity
–– 
<
––  
	OrderItem
––  )
>
––) *
(
––* +
)
––+ ,
.
—— 
HasOne
—— 
(
—— 
oi
—— 
=>
—— 
oi
——  
.
——  !
	Produkter
——! *
)
——* +
.
““ 
WithMany
““ 
(
““ 
)
““ 
.
”” 
HasForeignKey
”” 
(
”” 
oi
”” !
=>
””" $
oi
””% '
.
””' (
	ProductId
””( 1
)
””1 2
;
””2 3
modelBuilder
’’ 
.
’’ 
Entity
’’ 
<
’’  
Order
’’  %
>
’’% &
(
’’& '
)
’’' (
.
÷÷ 
HasOne
÷÷ 
(
÷÷ 
o
÷÷ 
=>
÷÷ 
o
÷÷ 
.
÷÷ 
Users
÷÷ $
)
÷÷$ %
.
◊◊ 
WithMany
◊◊ 
(
◊◊ 
)
◊◊ 
.
ÿÿ 
HasForeignKey
ÿÿ 
(
ÿÿ 
o
ÿÿ  
=>
ÿÿ! #
o
ÿÿ$ %
.
ÿÿ% &
UserId
ÿÿ& ,
)
ÿÿ, -
;
ÿÿ- .
}
ŸŸ 	
}
⁄⁄ 
}€€ µ&
YC:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Middleware\JwtMiddleware.cs
	namespace 	
MadkassenRestAPI
 
. 

Middleware %
{ 
public 

class 
JwtMiddleware 
( 
RequestDelegate .
next/ 3
,3 4
IConfiguration5 C
configurationD Q
,Q R
ILoggerS Z
<Z [
JwtMiddleware[ h
>h i
loggerj p
)p q
{ 
public		 
async		 
Task		 
Invoke		  
(		  !
HttpContext		! ,
context		- 4
)		4 5
{

 	
var 
token 
= 
context 
.  
Request  '
.' (
Headers( /
[/ 0
$str0 ?
]? @
.@ A
FirstOrDefaultA O
(O P
)P Q
?Q R
.R S
SplitS X
(X Y
$strY \
)\ ]
.] ^
Last^ b
(b c
)c d
;d e
if 
( 
! 
string 
. 
IsNullOrEmpty %
(% &
token& +
)+ ,
), -
{ 
try 
{ 
var 
tokenHandler $
=% &
new' *#
JwtSecurityTokenHandler+ B
(B C
)C D
;D E
var 
key 
= 
Encoding &
.& '
ASCII' ,
., -
GetBytes- 5
(5 6
configuration6 C
[C D
$strD W
]W X
)X Y
;Y Z
var  
validationParameters ,
=- .
new/ 2%
TokenValidationParameters3 L
{ $
ValidateIssuerSigningKey 0
=1 2
true3 7
,7 8
IssuerSigningKey (
=) *
new+ . 
SymmetricSecurityKey/ C
(C D
keyD G
)G H
,H I
ValidateIssuer &
=' (
true) -
,- .
ValidateAudience (
=) *
true+ /
,/ 0
ValidIssuer #
=$ %
configuration& 3
[3 4
$str4 H
]H I
,I J
ValidAudience %
=& '
configuration( 5
[5 6
$str6 L
]L M
,M N
	ClockSkew !
=" #
TimeSpan$ ,
., -
Zero- 1
} 
; 
var 
	principal !
=" #
tokenHandler$ 0
.0 1
ValidateToken1 >
(> ?
token? D
,D E 
validationParametersF Z
,Z [
out\ _
SecurityToken` m
validatedTokenn |
)| }
;} ~
var 
userId 
=  
	principal! *
.* +
Claims+ 1
.1 2
FirstOrDefault2 @
(@ A
cA B
=>C E
cF G
.G H
TypeH L
==M O
$strP U
)U V
?V W
.W X
ValueX ]
;] ^
var   
roles   
=   
	principal    )
.  ) *
Claims  * 0
.  0 1
FirstOrDefault  1 ?
(  ? @
c  @ A
=>  B D
c  E F
.  F G
Type  G K
==  L N
$str  O V
)  V W
?  W X
.  X Y
Value  Y ^
;  ^ _
if"" 
("" 
userId"" 
!="" !
null""" &
)""& '
{## 
context$$ 
.$$  
Items$$  %
[$$% &
$str$$& ,
]$$, -
=$$. /
userId$$0 6
;$$6 7
}%% 
if'' 
('' 
roles'' 
!=''  
null''! %
)''% &
{(( 
context)) 
.))  
Items))  %
[))% &
$str))& -
]))- .
=))/ 0
roles))1 6
.))6 7
Split))7 <
())< =
$char))= @
)))@ A
;))A B
}** 
}++ 
catch,, 
(,, 
	Exception,,  
ex,,! #
),,# $
{-- 
logger.. 
... 
LogError.. #
(..# $
$"..$ &
$str..& C
{..C D
ex..D F
...F G
Message..G N
}..N O
"..O P
)..P Q
;..Q R
}// 
}00 
await11 
next11 
(11 
context11 
)11 
;11  
}22 	
}33 
}44 ä∑
\C:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Controllers\UsersController.cs
	namespace

 	
MadkassenRestAPI


 
.

 
Controllers

 &
{ 
[ 
ApiController 
] 
[ 
Route 

(
 
$str 
) 
] 
public 

class 
UsersController  
(  ! 
ApplicationDbContext! 5
context6 =
,= >
IConfiguration? M
configurationN [
)[ \
:] ^
ControllerBase_ m
{ 
[ 	
HttpGet	 
] 
public 
async 
Task 
< 
ActionResult &
<& '
IEnumerable' 2
<2 3
Users3 8
>8 9
>9 :
>: ;
GetAllUsers< G
(G H
)H I
{ 	
var 
users 
= 
await 
context %
.% &
Users& +
. 
Select 
( 
u 
=> 
new  
Users! &
{ 
UserId 
= 
u 
. 
UserId %
,% &
UserName 
= 
u  
.  !
UserName! )
,) *
Email 
= 
u 
. 
Email #
,# $
	CreatedAt 
= 
u  !
.! "
	CreatedAt" +
,+ ,
	UpdatedAt 
= 
u  !
.! "
	UpdatedAt" +
,+ ,
Roles 
= 
u 
. 
Roles #
} 
) 
. 
ToListAsync 
( 
) 
; 
return   
Ok   
(   
users   
)   
;   
}!! 	
[$$ 	
HttpGet$$	 
($$ 
$str$$ 
)$$ 
]$$ 
public%% 
async%% 
Task%% 
<%% 
ActionResult%% &
<%%& '
Users%%' ,
>%%, -
>%%- .
GetUserById%%/ :
(%%: ;
int%%; >
id%%? A
)%%A B
{&& 	
var'' 
user'' 
='' 
await'' 
context'' $
.''$ %
Users''% *
.(( 
Where(( 
((( 
u(( 
=>(( 
u(( 
.(( 
UserId(( $
==((% '
id((( *
)((* +
.)) 
Select)) 
()) 
u)) 
=>)) 
new))  
Users))! &
{** 
UserId++ 
=++ 
u++ 
.++ 
UserId++ %
,++% &
UserName,, 
=,, 
u,,  
.,,  !
UserName,,! )
,,,) *
Email-- 
=-- 
u-- 
.-- 
Email-- #
,--# $
	CreatedAt.. 
=.. 
u..  !
...! "
	CreatedAt.." +
,..+ ,
	UpdatedAt// 
=// 
u//  !
.//! "
	UpdatedAt//" +
,//+ ,
Roles00 
=00 
u00 
.00 
Roles00 #
}11 
)11 
.22 
FirstOrDefaultAsync22 $
(22$ %
)22% &
;22& '
if44 
(44 
user44 
==44 
null44 
)44 
{55 
return66 
NotFound66 
(66  
)66  !
;66! "
}77 
return99 
Ok99 
(99 
user99 
)99 
;99 
}:: 	
[<< 	
HttpPut<<	 
(<< 
$str<< !
)<<! "
]<<" #
public== 
async== 
Task== 
<== 
IActionResult== '
>==' (
UpdateUserProfile==) :
(==: ;
[==; <
FromBody==< D
]==D E$
UpdateUserProfileRequest==F ^
updateRequest==_ l
)==l m
{>> 	
var@@ 
token@@ 
=@@ 
Request@@ 
.@@  
Headers@@  '
[@@' (
$str@@( 7
]@@7 8
.@@8 9
FirstOrDefault@@9 G
(@@G H
)@@H I
?@@I J
.@@J K
Split@@K P
(@@P Q
$str@@Q T
)@@T U
.@@U V
Last@@V Z
(@@Z [
)@@[ \
;@@\ ]
ifAA 
(AA 
stringAA 
.AA 
IsNullOrEmptyAA $
(AA$ %
tokenAA% *
)AA* +
)AA+ ,
{BB 
returnCC 
UnauthorizedCC #
(CC# $
newCC$ '
{CC( )
MessageCC* 1
=CC2 3
$strCC4 H
}CCI J
)CCJ K
;CCK L
}DD 
varGG 
userProfileGG 
=GG 
awaitGG ##
GetUserProfileFromTokenGG$ ;
(GG; <
tokenGG< A
)GGA B
;GGB C
ifHH 
(HH 
userProfileHH 
==HH 
nullHH #
)HH# $
{II 
returnJJ 
UnauthorizedJJ #
(JJ# $
newJJ$ '
{JJ( )
MessageJJ* 1
=JJ2 3
$strJJ4 O
}JJP Q
)JJQ R
;JJR S
}KK 
varNN 
userNN 
=NN 
awaitNN 
contextNN $
.NN$ %
UsersNN% *
.NN* +
	FindAsyncNN+ 4
(NN4 5
intNN5 8
.NN8 9
ParseNN9 >
(NN> ?
userProfileNN? J
.NNJ K
UserIdNNK Q
)NNQ R
)NNR S
;NNS T
ifOO 
(OO 
userOO 
==OO 
nullOO 
)OO 
{PP 
returnQQ 
NotFoundQQ 
(QQ  
newQQ  #
{QQ$ %
MessageQQ& -
=QQ. /
$strQQ0 A
}QQB C
)QQC D
;QQD E
}RR 
ifUU 
(UU 
!UU 
stringUU 
.UU 
IsNullOrEmptyUU %
(UU% &
updateRequestUU& 3
.UU3 4
UserNameUU4 <
)UU< =
&&UU> @
updateRequestUUA N
.UUN O
UserNameUUO W
!=UUX Z
$strUU[ c
&&UUd f
updateRequestVV 
.VV 
UserNameVV &
!=VV' )
userVV* .
.VV. /
UserNameVV/ 7
)VV7 8
{WW 
userXX 
.XX 
UserNameXX 
=XX 
updateRequestXX  -
.XX- .
UserNameXX. 6
;XX6 7
}YY 
if\\ 
(\\ 
!\\ 
string\\ 
.\\ 
IsNullOrEmpty\\ %
(\\% &
updateRequest\\& 3
.\\3 4
Email\\4 9
)\\9 :
&&\\; =
updateRequest\\> K
.\\K L
Email\\L Q
!=\\R T
$str\\U ]
&&\\^ `
updateRequest]] 
.]] 
Email]] #
!=]]$ &
user]]' +
.]]+ ,
Email]], 1
)]]1 2
{^^ 
var__ !
existingUserWithEmail__ )
=__* +
await`` 
context`` !
.``! "
Users``" '
.``' (
FirstOrDefaultAsync``( ;
(``; <
u``< =
=>``> @
u``A B
.``B C
Email``C H
==``I K
updateRequest``L Y
.``Y Z
Email``Z _
)``_ `
;``` a
ifaa 
(aa !
existingUserWithEmailaa )
!=aa* ,
nullaa- 1
)aa1 2
{bb 
returncc 

BadRequestcc %
(cc% &
newcc& )
{cc* +
Messagecc, 3
=cc4 5
$strcc6 P
}ccQ R
)ccR S
;ccS T
}dd 
userff 
.ff 
Emailff 
=ff 
updateRequestff *
.ff* +
Emailff+ 0
;ff0 1
}gg 
ifjj 
(jj 
!jj 
stringjj 
.jj 
IsNullOrEmptyjj %
(jj% &
updateRequestjj& 3
.jj3 4
OldPasswordjj4 ?
)jj? @
&&jjA C
updateRequestjjD Q
.jjQ R
OldPasswordjjR ]
!=jj^ `
$strjja i
)jji j
{kk 
ifll 
(ll 
!ll 
VerifyPasswordll #
(ll# $
updateRequestll$ 1
.ll1 2
OldPasswordll2 =
,ll= >
userll? C
.llC D
PasswordHashllD P
)llP Q
)llQ R
{mm 
returnnn 

BadRequestnn %
(nn% &
newnn& )
{nn* +
Messagenn, 3
=nn4 5
$strnn6 O
}nnP Q
)nnQ R
;nnR S
}oo 
}pp 
ifrr 
(rr 
!rr 
stringrr 
.rr 
IsNullOrEmptyrr %
(rr% &
updateRequestrr& 3
.rr3 4
NewPasswordrr4 ?
)rr? @
&&rrA C
updateRequestrrD Q
.rrQ R
NewPasswordrrR ]
!=rr^ `
$strrra i
)rri j
{ss 
usertt 
.tt 
PasswordHashtt !
=tt" #
HashPasswordtt$ 0
(tt0 1
updateRequesttt1 >
.tt> ?
NewPasswordtt? J
)ttJ K
;ttK L
}uu 
ifxx 
(xx 
updateRequestxx 
.xx 
UserNamexx &
==xx' )
$strxx* 2
&&xx3 5
updateRequestxx6 C
.xxC D
EmailxxD I
==xxJ L
$strxxM U
&&xxV X
updateRequestyy 
.yy 
OldPasswordyy )
==yy* ,
$stryy- 5
&&yy6 8
updateRequestyy9 F
.yyF G
NewPasswordyyG R
==yyS U
$stryyV ^
)yy^ _
{zz 
return{{ 

BadRequest{{ !
({{! "
new{{" %
{{{& '
Message{{( /
={{0 1
$str{{2 U
}{{V W
){{W X
;{{X Y
}|| 
user 
. 
	UpdatedAt 
= 
DateTime %
.% &
UtcNow& ,
;, -
await
ÇÇ 
context
ÇÇ 
.
ÇÇ 
SaveChangesAsync
ÇÇ *
(
ÇÇ* +
)
ÇÇ+ ,
;
ÇÇ, -
var
ÖÖ 
newToken
ÖÖ 
=
ÖÖ 

JwtBuilder
ÖÖ %
.
ÖÖ% &
Create
ÖÖ& ,
(
ÖÖ, -
)
ÖÖ- .
.
ÜÜ 
WithAlgorithm
ÜÜ 
(
ÜÜ 
new
ÜÜ "!
HMACSHA256Algorithm
ÜÜ# 6
(
ÜÜ6 7
)
ÜÜ7 8
)
ÜÜ8 9
.
áá 

WithSecret
áá 
(
áá 
configuration
áá )
[
áá) *
$str
áá* =
]
áá= >
)
áá> ?
.
àà 
Subject
àà 
(
àà 
user
àà 
.
àà 
UserId
àà $
.
àà$ %
ToString
àà% -
(
àà- .
)
àà. /
)
àà/ 0
.
ââ 
Issuer
ââ 
(
ââ 
configuration
ââ %
[
ââ% &
$str
ââ& :
]
ââ: ;
)
ââ; <
.
ää 
Audience
ää 
(
ää 
configuration
ää '
[
ää' (
$str
ää( >
]
ää> ?
)
ää? @
.
ãã 
IssuedAt
ãã 
(
ãã 
DateTimeOffset
ãã (
.
ãã( )
Now
ãã) ,
.
ãã, -
DateTime
ãã- 5
)
ãã5 6
.
åå 
ExpirationTime
åå 
(
åå  
DateTimeOffset
åå  .
.
åå. /
Now
åå/ 2
.
åå2 3
AddHours
åå3 ;
(
åå; <
$num
åå< =
)
åå= >
.
åå> ?
DateTime
åå? G
)
ååG H
.
çç 
	NotBefore
çç 
(
çç 
DateTimeOffset
çç )
.
çç) *
Now
çç* -
.
çç- .
DateTime
çç. 6
)
çç6 7
.
éé 
Id
éé 
(
éé 
Guid
éé 
.
éé 
NewGuid
éé  
(
éé  !
)
éé! "
.
éé" #
ToString
éé# +
(
éé+ ,
)
éé, -
)
éé- .
.
èè 
Encode
èè 
(
èè 
)
èè 
;
èè 
return
íí 
Ok
íí 
(
íí 
new
íí 
{
íí 
Message
íí #
=
íí$ %
$str
íí& E
,
ííE F
Token
ííG L
=
ííM N
newToken
ííO W
}
ííX Y
)
ííY Z
;
ííZ [
}
ìì 	
[
óó 	
HttpGet
óó	 
(
óó 
$str
óó 
)
óó 
]
óó 
public
òò 
async
òò 
Task
òò 
<
òò 
IActionResult
òò '
>
òò' (

GetProfile
òò) 3
(
òò3 4
)
òò4 5
{
ôô 	
var
öö 
token
öö 
=
öö 
Request
öö 
.
öö  
Headers
öö  '
[
öö' (
$str
öö( 7
]
öö7 8
.
öö8 9
FirstOrDefault
öö9 G
(
ööG H
)
ööH I
?
ööI J
.
ööJ K
Split
ööK P
(
ööP Q
$str
ööQ T
)
ööT U
.
ööU V
Last
ööV Z
(
ööZ [
)
öö[ \
;
öö\ ]
if
õõ 
(
õõ 
string
õõ 
.
õõ 
IsNullOrEmpty
õõ $
(
õõ$ %
token
õõ% *
)
õõ* +
)
õõ+ ,
{
úú 
return
ùù 
Unauthorized
ùù #
(
ùù# $
new
ùù$ '
{
ùù( )
Message
ùù* 1
=
ùù2 3
$str
ùù4 H
}
ùùI J
)
ùùJ K
;
ùùK L
}
ûû 
try
†† 
{
°° 
var
¢¢ 
userProfile
¢¢ 
=
¢¢  !
await
¢¢" '%
GetUserProfileFromToken
¢¢( ?
(
¢¢? @
token
¢¢@ E
)
¢¢E F
;
¢¢F G
if
££ 
(
££ 
userProfile
££ 
==
££  "
null
££# '
)
££' (
{
§§ 
return
•• 
Unauthorized
•• '
(
••' (
new
••( +
{
••, -
Message
••. 5
=
••6 7
$str
••8 S
}
••T U
)
••U V
;
••V W
}
¶¶ 
var
®® 
user
®® 
=
®® 
await
®®  
context
®®! (
.
®®( )
Users
®®) .
.
©© 
Where
©© 
(
©© 
u
©© 
=>
©© 
u
©©  !
.
©©! "
UserId
©©" (
.
©©( )
ToString
©©) 1
(
©©1 2
)
©©2 3
==
©©4 6
userProfile
©©7 B
.
©©B C
UserId
©©C I
)
©©I J
.
™™ !
FirstOrDefaultAsync
™™ (
(
™™( )
)
™™) *
;
™™* +
if
¨¨ 
(
¨¨ 
user
¨¨ 
==
¨¨ 
null
¨¨  
)
¨¨  !
{
≠≠ 
return
ÆÆ 
NotFound
ÆÆ #
(
ÆÆ# $
new
ÆÆ$ '
{
ÆÆ( )
Message
ÆÆ* 1
=
ÆÆ2 3
$str
ÆÆ4 E
}
ÆÆF G
)
ÆÆG H
;
ÆÆH I
}
ØØ 
var
±± 
userDetails
±± 
=
±±  !
new
±±" %
{
≤≤ 
user
≥≥ 
.
≥≥ 
UserId
≥≥ 
,
≥≥  
user
¥¥ 
.
¥¥ 
UserName
¥¥ !
,
¥¥! "
user
µµ 
.
µµ 
Email
µµ 
,
µµ 
user
∂∂ 
.
∂∂ 
	CreatedAt
∂∂ "
,
∂∂" #
user
∑∑ 
.
∑∑ 
	UpdatedAt
∑∑ "
,
∑∑" #
user
∏∏ 
.
∏∏ 
Roles
∏∏ 
}
ππ 
;
ππ 
return
ªª 
Ok
ªª 
(
ªª 
userDetails
ªª %
)
ªª% &
;
ªª& '
}
ºº 
catch
ΩΩ 
(
ΩΩ 
	Exception
ΩΩ 
ex
ΩΩ 
)
ΩΩ  
{
ææ 
return
øø 

StatusCode
øø !
(
øø! "
$num
øø" %
,
øø% &
new
øø' *
{
øø+ ,
Message
øø- 4
=
øø5 6
$str
øø7 V
,
øøV W
Details
øøX _
=
øø` a
ex
øøb d
.
øød e
Message
øøe l
}
øøm n
)
øøn o
;
øøo p
}
¿¿ 
}
¡¡ 	
private
√√ 
async
√√ 
Task
√√ 
<
√√ 
UserProfile
√√ &
>
√√& '%
GetUserProfileFromToken
√√( ?
(
√√? @
string
√√@ F
token
√√G L
)
√√L M
{
ƒƒ 	
var
≈≈ 
tokenHandler
≈≈ 
=
≈≈ 
new
≈≈ "%
JwtSecurityTokenHandler
≈≈# :
(
≈≈: ;
)
≈≈; <
;
≈≈< =
try
«« 
{
»» 
var
…… 
jwtToken
…… 
=
…… 
tokenHandler
…… +
.
……+ ,
	ReadToken
……, 5
(
……5 6
token
……6 ;
)
……; <
as
……= ?
JwtSecurityToken
……@ P
;
……P Q
var
ÀÀ 
userId
ÀÀ 
=
ÀÀ 
jwtToken
ÀÀ %
?
ÀÀ% &
.
ÀÀ& '
Claims
ÀÀ' -
.
ÀÀ- .
FirstOrDefault
ÀÀ. <
(
ÀÀ< =
c
ÀÀ= >
=>
ÀÀ? A
c
ÀÀB C
.
ÀÀC D
Type
ÀÀD H
==
ÀÀI K
$str
ÀÀL Q
)
ÀÀQ R
?
ÀÀR S
.
ÀÀS T
Value
ÀÀT Y
;
ÀÀY Z
if
ÕÕ 
(
ÕÕ 
userId
ÕÕ 
==
ÕÕ 
null
ÕÕ "
)
ÕÕ" #
{
ŒŒ 
return
œœ 
null
œœ 
;
œœ  
}
–– 
return
““ 
new
““ 
UserProfile
““ &
{
”” 
UserId
‘‘ 
=
‘‘ 
userId
‘‘ #
}
’’ 
;
’’ 
}
÷÷ 
catch
◊◊ 
(
◊◊ 
	Exception
◊◊ 
)
◊◊ 
{
ÿÿ 
return
ŸŸ 
null
ŸŸ 
;
ŸŸ 
}
⁄⁄ 
}
€€ 	
[
‹‹ 	
HttpPost
‹‹	 
]
‹‹ 
public
›› 
async
›› 
Task
›› 
<
›› 
ActionResult
›› &
<
››& '
User
››' +
>
››+ ,
>
››, -

CreateUser
››. 8
(
››8 9
User
››9 =
user
››> B
)
››B C
{
ﬁﬁ 	
if
ﬂﬂ 
(
ﬂﬂ 
string
ﬂﬂ 
.
ﬂﬂ 
IsNullOrEmpty
ﬂﬂ $
(
ﬂﬂ$ %
user
ﬂﬂ% )
.
ﬂﬂ) *
UserName
ﬂﬂ* 2
)
ﬂﬂ2 3
)
ﬂﬂ3 4
{
‡‡ 
return
·· 

BadRequest
·· !
(
··! "
$str
··" 9
)
··9 :
;
··: ;
}
‚‚ 
if
‰‰ 
(
‰‰ 
string
‰‰ 
.
‰‰ 
IsNullOrEmpty
‰‰ $
(
‰‰$ %
user
‰‰% )
.
‰‰) *
Email
‰‰* /
)
‰‰/ 0
)
‰‰0 1
{
ÂÂ 
return
ÊÊ 

BadRequest
ÊÊ !
(
ÊÊ! "
$str
ÊÊ" 6
)
ÊÊ6 7
;
ÊÊ7 8
}
ÁÁ 
if
ÈÈ 
(
ÈÈ 
string
ÈÈ 
.
ÈÈ 
IsNullOrEmpty
ÈÈ $
(
ÈÈ$ %
user
ÈÈ% )
.
ÈÈ) *
PasswordHash
ÈÈ* 6
)
ÈÈ6 7
)
ÈÈ7 8
{
ÍÍ 
return
ÎÎ 

BadRequest
ÎÎ !
(
ÎÎ! "
$str
ÎÎ" 9
)
ÎÎ9 :
;
ÎÎ: ;
}
ÏÏ 
var
ÓÓ 
existingUser
ÓÓ 
=
ÓÓ 
await
ÓÓ $
context
ÓÓ% ,
.
ÓÓ, -
Users
ÓÓ- 2
.
ÓÓ2 3!
FirstOrDefaultAsync
ÓÓ3 F
(
ÓÓF G
u
ÓÓG H
=>
ÓÓI K
u
ÓÓL M
.
ÓÓM N
Email
ÓÓN S
==
ÓÓT V
user
ÓÓW [
.
ÓÓ[ \
Email
ÓÓ\ a
)
ÓÓa b
;
ÓÓb c
if
ÔÔ 
(
ÔÔ 
existingUser
ÔÔ 
!=
ÔÔ 
null
ÔÔ  $
)
ÔÔ$ %
{
 
return
ÒÒ 

BadRequest
ÒÒ !
(
ÒÒ! "
$str
ÒÒ" <
)
ÒÒ< =
;
ÒÒ= >
}
ÚÚ 
user
ÙÙ 
.
ÙÙ 
	CreatedAt
ÙÙ 
=
ÙÙ 
DateTime
ÙÙ %
.
ÙÙ% &
UtcNow
ÙÙ& ,
;
ÙÙ, -
user
ıı 
.
ıı 
	UpdatedAt
ıı 
=
ıı 
DateTime
ıı %
.
ıı% &
UtcNow
ıı& ,
;
ıı, -
if
˜˜ 
(
˜˜ 
string
˜˜ 
.
˜˜ 
IsNullOrEmpty
˜˜ $
(
˜˜$ %
user
˜˜% )
.
˜˜) *
UserName
˜˜* 2
)
˜˜2 3
)
˜˜3 4
{
¯¯ 
user
˘˘ 
.
˘˘ 
UserName
˘˘ 
=
˘˘ 
user
˘˘  $
.
˘˘$ %
Email
˘˘% *
;
˘˘* +
}
˙˙ 
user
˝˝ 
.
˝˝ 
PasswordHash
˝˝ 
=
˝˝ 
HashPassword
˝˝  ,
(
˝˝, -
user
˝˝- 1
.
˝˝1 2
PasswordHash
˝˝2 >
)
˝˝> ?
;
˝˝? @
context
ˇˇ 
.
ˇˇ 
Users
ˇˇ 
.
ˇˇ 
Add
ˇˇ 
(
ˇˇ 
user
ˇˇ "
)
ˇˇ" #
;
ˇˇ# $
await
ÄÄ 
context
ÄÄ 
.
ÄÄ 
SaveChangesAsync
ÄÄ *
(
ÄÄ* +
)
ÄÄ+ ,
;
ÄÄ, -
return
ÇÇ 
CreatedAtAction
ÇÇ "
(
ÇÇ" #
nameof
ÇÇ# )
(
ÇÇ) *
GetUserById
ÇÇ* 5
)
ÇÇ5 6
,
ÇÇ6 7
new
ÇÇ8 ;
{
ÇÇ< =
id
ÇÇ> @
=
ÇÇA B
user
ÇÇC G
.
ÇÇG H
UserId
ÇÇH N
}
ÇÇO P
,
ÇÇP Q
user
ÇÇR V
)
ÇÇV W
;
ÇÇW X
}
ÉÉ 	
private
ÖÖ 
string
ÖÖ 
HashPassword
ÖÖ #
(
ÖÖ# $
string
ÖÖ$ *
password
ÖÖ+ 3
)
ÖÖ3 4
{
ÜÜ 	
return
áá 
BCrypt
áá 
.
áá 
Net
áá 
.
áá 
BCrypt
áá $
.
áá$ %
HashPassword
áá% 1
(
áá1 2
password
áá2 :
)
áá: ;
;
áá; <
}
àà 	
private
ää 
bool
ää 
VerifyPassword
ää #
(
ää# $
string
ää$ *
password
ää+ 3
,
ää3 4
string
ää5 ;
hash
ää< @
)
ää@ A
{
ãã 	
return
åå 
BCrypt
åå 
.
åå 
Net
åå 
.
åå 
BCrypt
åå $
.
åå$ %
Verify
åå% +
(
åå+ ,
password
åå, 4
,
åå4 5
hash
åå6 :
)
åå: ;
;
åå; <
}
çç 	
}
éé 
}èè ˛L
^C:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Controllers\ProductController.cs
	namespace		 	
MadkassenRestAPI		
 
.		 
Controllers		 &
{

 
[ 
Route 

(
 
$str 
) 
] 
[ 
ApiController 
] 
public 

class 
ProductController "
(" #
ProductService# 1
productService2 @
,@ A 
ApplicationDbContextB V
contextW ^
)^ _
: 	
ControllerBase
 
{ 
[ 	
HttpGet	 
] 
public 
async 
Task 
< 
ActionResult &
<& '
IEnumerable' 2
<2 3
	Produkter3 <
>< =
>= >
>> ?
GetAllProducts@ N
(N O
)O P
{ 	
var 
products 
= 
await  
context! (
.( )
	Produkter) 2
.2 3
ToListAsync3 >
(> ?
)? @
;@ A
foreach 
( 
var 
product  
in! #
products$ ,
), -
{ 
if 
( 
string 
. 
IsNullOrEmpty (
(( )
product) 0
.0 1
ImageUrl1 9
)9 :
): ;
{ 
product 
. 
ImageUrl $
=% &
$str F
;F G
} 
} 
return 
Ok 
( 
products 
) 
;  
} 	
[!! 	
HttpPost!!	 
]!! 
public"" 
async"" 
Task"" 
<"" 
ActionResult"" &
<""& '
	Produkter""' 0
>""0 1
>""1 2

AddProduct""3 =
(""= >
	Produkter""> G
product""H O
)""O P
{## 	
var$$ 
userId$$ 
=$$ 
User$$ 
.$$ 
	FindFirst$$ '
($$' (

ClaimTypes$$( 2
.$$2 3
NameIdentifier$$3 A
)$$A B
?$$B C
.$$C D
Value$$D I
;$$I J
if&& 
(&& 
string&& 
.&& 
IsNullOrEmpty&& $
(&&$ %
userId&&% +
)&&+ ,
)&&, -
{'' 
return(( 
Unauthorized(( #
(((# $
new(($ '
{((( )
message((* 1
=((2 3
$str((4 Y
}((Z [
)(([ \
;((\ ]
})) 
if++ 
(++ 
string++ 
.++ 
IsNullOrEmpty++ $
(++$ %
product++% ,
.++, -
ImageUrl++- 5
)++5 6
||++7 9
product++: A
.++A B
ImageUrl++B J
==++K M
$str++N V
)++V W
{,, 
product-- 
.-- 
ImageUrl--  
=--! "
null--# '
;--' (
}.. 
context00 
.00 
	Produkter00 
.00 
Add00 !
(00! "
product00" )
)00) *
;00* +
await11 
context11 
.11 
SaveChangesAsync11 *
(11* +
)11+ ,
;11, -
return33 
CreatedAtAction33 "
(33" #
nameof33# )
(33) *

GetProduct33* 4
)334 5
,335 6
new337 :
{33; <
id33= ?
=33@ A
product33B I
.33I J
	ProductId33J S
}33T U
,33U V
product33W ^
)33^ _
;33_ `
}44 	
[77 	
HttpGet77	 
(77 
$str77 
)77 
]77 
public88 
async88 
Task88 
<88 
ActionResult88 &
<88& '
	Produkter88' 0
>880 1
>881 2

GetProduct883 =
(88= >
int88> A
id88B D
)88D E
{99 	
var;; 
product;; 
=;; 
await;; 
productService;;  .
.;;. /
GetProductByIdAsync;;/ B
(;;B C
id;;C E
);;E F
;;;F G
if== 
(== 
product== 
==== 
null== 
)==  
{>> 
return?? 
NotFound?? 
(??  
)??  !
;??! "
}@@ 
returnBB 
OkBB 
(BB 
productBB 
)BB 
;BB 
}CC 	
[EE 	
HttpPutEE	 
(EE 
$strEE 
)EE 
]EE 
publicFF 
asyncFF 
TaskFF 
<FF 
IActionResultFF '
>FF' (
UpdateProductStockFF) ;
(FF; <
intFF< ?
idFF@ B
,FFB C
[FFD E
FromBodyFFE M
]FFM N
UpdateStockRequestFFO a
requestFFb i
)FFi j
{GG 	
varHH 
updatedProductHH 
=HH  
awaitHH! &
productServiceHH' 5
.HH5 6#
UpdateProductStockAsyncHH6 M
(HHM N
idHHN P
,HHP Q
requestHHR Y
.HHY Z
QuantityHHZ b
)HHb c
;HHc d
ifJJ 
(JJ 
updatedProductJJ 
==JJ !
nullJJ" &
)JJ& '
{KK 
returnLL 

BadRequestLL !
(LL! "
$strLL" >
)LL> ?
;LL? @
}MM 
returnOO 
OkOO 
(OO 
updatedProductOO $
)OO$ %
;OO% &
}PP 	
[RR 	
HttpGetRR	 
(RR 
$strRR (
)RR( )
]RR) *
publicSS 
asyncSS 
TaskSS 
<SS 
IActionResultSS '
>SS' (!
GetProductsByCategorySS) >
(SS> ?
intSS? B

categoryIdSSC M
)SSM N
{TT 	
ifVV 
(VV 

categoryIdVV 
<=VV 
$numVV 
)VV  
{WW 
returnXX 

BadRequestXX !
(XX! "
newXX" %
{XX& '
messageXX( /
=XX0 1
$strXX2 G
}XXH I
)XXI J
;XXJ K
}YY 
var[[ 
productsQuery[[ 
=[[ 
context[[  '
.[[' (
	Produkter[[( 1
.\\ 
Where\\ 
(\\ 
p\\ 
=>\\ 
p\\ 
.\\ 

CategoryId\\ (
==\\) +

categoryId\\, 6
)\\6 7
;\\7 8
productsQuery^^ 
=^^ 
productsQuery^^ )
.^^) *
OrderBy^^* 1
(^^1 2
p^^2 3
=>^^4 6
p^^7 8
.^^8 9
Price^^9 >
)^^> ?
;^^? @
var`` 
products`` 
=`` 
await``  
productsQuery``! .
.aa 
ToListAsyncaa 
(aa 
)aa 
;aa 
ifcc 
(cc 
productscc 
==cc 
nullcc  
||cc! #
productscc$ ,
.cc, -
Countcc- 2
==cc3 5
$numcc6 7
)cc7 8
{dd 
returnee 
NotFoundee 
(ee  
newee  #
{ee$ %
messageee& -
=ee. /
$stree0 T
}eeU V
)eeV W
;eeW X
}ff 
returnii 
Okii 
(ii 
productsii 
)ii 
;ii  
}jj 	
[mm 	

HttpDeletemm	 
(mm 
$strmm 
)mm 
]mm 
publicnn 
asyncnn 
Tasknn 
<nn 
IActionResultnn '
>nn' (
DeleteProductnn) 6
(nn6 7
intnn7 :
Idnn; =
)nn= >
{oo 	
varpp 
userIdpp 
=pp 
Userpp 
.pp 
	FindFirstpp '
(pp' (

ClaimTypespp( 2
.pp2 3
NameIdentifierpp3 A
)ppA B
?ppB C
.ppC D
ValueppD I
;ppI J
iftt 
(tt 
stringtt 
.tt 
IsNullOrEmptytt $
(tt$ %
userIdtt% +
)tt+ ,
)tt, -
{uu 
returnvv 
Unauthorizedvv #
(vv# $
newvv$ '
{vv( )
messagevv* 1
=vv2 3
$strvv4 Y
}vvZ [
)vv[ \
;vv\ ]
}ww 
var~~ 
product~~ 
=~~ 
await~~ 
context~~  '
.~~' (
	Produkter~~( 1
.~~1 2
	FindAsync~~2 ;
(~~; <
Id~~< >
)~~> ?
;~~? @
if 
( 
product 
== 
null 
)  
{
ÄÄ 
return
ÅÅ 
NotFound
ÅÅ 
(
ÅÅ  
new
ÅÅ  #
{
ÅÅ$ %
message
ÅÅ& -
=
ÅÅ. /
$str
ÅÅ0 C
}
ÅÅD E
)
ÅÅE F
;
ÅÅF G
}
ÇÇ 
context
ÉÉ 
.
ÉÉ 
	Produkter
ÉÉ 
.
ÉÉ 
Remove
ÉÉ $
(
ÉÉ$ %
product
ÉÉ% ,
)
ÉÉ, -
;
ÉÉ- .
await
ÑÑ 
context
ÑÑ 
.
ÑÑ 
SaveChangesAsync
ÑÑ *
(
ÑÑ* +
)
ÑÑ+ ,
;
ÑÑ, -
return
ÖÖ 
	NoContent
ÖÖ 
(
ÖÖ 
)
ÖÖ 
;
ÖÖ 
}
ÜÜ 	
}
áá 
}àà ˜U
\C:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Controllers\OrderController.cs
	namespace 	
MadkassenRestAPI
 
. 
Controllers &
{ 
[ 
ApiController 
] 
[		 
Route		 

(		
 
$str		 
)		 
]		 
public

 

class

 
OrderController

  
(

  !
OrderService 
orderService !
,! "
IConfiguration 
configuration $
,$ %
ILogger 
< 
OrderController 
>  
logger! '
)' (
: 	
ControllerBase
 
{ 
private 
readonly 
OrderService %
_orderService& 3
=4 5
orderService 
?? 
throw !
new" %!
ArgumentNullException& ;
(; <
nameof< B
(B C
orderServiceC O
)O P
)P Q
;Q R
private 
readonly 
IConfiguration '
_configuration( 6
=7 8
configuration 
?? 
throw "
new# &!
ArgumentNullException' <
(< =
nameof= C
(C D
configurationD Q
)Q R
)R S
;S T
private 
readonly 
ILogger  
<  !
OrderController! 0
>0 1
_logger2 9
=: ;
logger< B
??C E
throwF K
newL O!
ArgumentNullExceptionP e
(e f
nameoff l
(l m
loggerm s
)s t
)t u
;u v
[ 	
HttpPost	 
( 
$str 
) 
] 
public 
async 
Task 
< 
IActionResult '
>' (
Checkout) 1
(1 2
)2 3
{ 	
try 
{ 
var 
token 
= 
Request #
.# $
Headers$ +
[+ ,
$str, ;
]; <
.< =
FirstOrDefault= K
(K L
)L M
?M N
.N O
SplitO T
(T U
$strU X
)X Y
.Y Z
LastZ ^
(^ _
)_ `
;` a
if 
( 
string 
. 
IsNullOrEmpty (
(( )
token) .
). /
)/ 0
{ 
_logger   
.   

LogWarning   &
(  & '
$str  ' ;
)  ; <
;  < =
return!! 
Unauthorized!! '
(!!' (
new!!( +
{!!, -
Message!!. 5
=!!6 7
$str!!8 L
}!!M N
)!!N O
;!!O P
}"" 
var$$ 
userProfile$$ 
=$$  !
await$$" '#
GetUserProfileFromToken$$( ?
($$? @
token$$@ E
)$$E F
;$$F G
if%% 
(%% 
userProfile%% 
==%%  "
null%%# '
)%%' (
{&& 
_logger'' 
.'' 

LogWarning'' &
(''& '
$str''' B
)''B C
;''C D
return(( 
Unauthorized(( '
(((' (
new((( +
{((, -
Message((. 5
=((6 7
$str((8 S
}((T U
)((U V
;((V W
})) 
var++ 
userId++ 
=++ 
int++  
.++  !
Parse++! &
(++& '
userProfile++' 2
.++2 3
UserId++3 9
)++9 :
;++: ;
var.. 
orderId.. 
=.. 
await.. #
_orderService..$ 1
...1 2
CreateOrderAsync..2 B
(..B C
userId..C I
)..I J
;..J K
return00 
Ok00 
(00 
new00 
{00 
Message00  '
=00( )
$str00* F
,00F G
OrderId00H O
=00P Q
orderId00R Y
}00Z [
)00[ \
;00\ ]
}11 
catch22 
(22 
	Exception22 
ex22 
)22  
{33 
_logger44 
.44 
LogError44  
(44  !
$"44! #
$str44# 6
{446 7
ex447 9
.449 :
Message44: A
}44A B
"44B C
,44C D
ex44E G
)44G H
;44H I
return55 

BadRequest55 !
(55! "
new55" %
{55& '
Message55( /
=550 1
$str552 ;
+55< =
ex55> @
.55@ A
Message55A H
}55I J
)55J K
;55K L
}66 
}77 	
[99 	
HttpGet99	 
(99 
$str99 $
)99$ %
]99% &
public:: 
async:: 
Task:: 
<:: 
IActionResult:: '
>::' ( 
GetTopProductsByUser::) =
(::= >
)::> ?
{;; 	
try<< 
{== 
var>> 
token>> 
=>> 
Request>> #
.>># $
Headers>>$ +
[>>+ ,
$str>>, ;
]>>; <
.>>< =
FirstOrDefault>>= K
(>>K L
)>>L M
?>>M N
.>>N O
Split>>O T
(>>T U
$str>>U X
)>>X Y
.>>Y Z
Last>>Z ^
(>>^ _
)>>_ `
;>>` a
if?? 
(?? 
string?? 
.?? 
IsNullOrEmpty?? (
(??( )
token??) .
)??. /
)??/ 0
{@@ 
_loggerAA 
.AA 

LogWarningAA &
(AA& '
$strAA' ;
)AA; <
;AA< =
returnBB 
UnauthorizedBB '
(BB' (
newBB( +
{BB, -
MessageBB. 5
=BB6 7
$strBB8 L
}BBM N
)BBN O
;BBO P
}CC 
varEE 
userProfileEE 
=EE  !
awaitEE" '#
GetUserProfileFromTokenEE( ?
(EE? @
tokenEE@ E
)EEE F
;EEF G
ifFF 
(FF 
userProfileFF 
==FF  "
nullFF# '
)FF' (
{GG 
_loggerHH 
.HH 

LogWarningHH &
(HH& '
$strHH' B
)HHB C
;HHC D
returnII 
UnauthorizedII '
(II' (
newII( +
{II, -
MessageII. 5
=II6 7
$strII8 S
}IIT U
)IIU V
;IIV W
}JJ 
varLL 
userIdLL 
=LL 
intLL  
.LL  !
ParseLL! &
(LL& '
userProfileLL' 2
.LL2 3
UserIdLL3 9
)LL9 :
;LL: ;
varMM 
productsMM 
=MM 
awaitMM $
_orderServiceMM% 2
.MM2 3%
GetTopProductsByUserAsyncMM3 L
(MML M
userIdMMM S
,MMS T
$numMMU W
)MMW X
;MMX Y
returnOO 
OkOO 
(OO 
productsOO "
)OO" #
;OO# $
}PP 
catchQQ 
(QQ 
	ExceptionQQ 
exQQ 
)QQ  
{RR 
_loggerSS 
.SS 
LogErrorSS  
(SS  !
$"SS! #
$strSS# H
{SSH I
exSSI K
.SSK L
MessageSSL S
}SSS T
"SST U
,SSU V
exSSW Y
)SSY Z
;SSZ [
returnTT 

BadRequestTT !
(TT! "
newTT" %
{TT& '
MessageTT( /
=TT0 1
$strTT2 ;
+TT< =
exTT> @
.TT@ A
MessageTTA H
}TTI J
)TTJ K
;TTK L
}UU 
}VV 	
[XX 	
HttpGetXX	 
(XX 
$strXX %
)XX% &
]XX& '
publicYY 
asyncYY 
TaskYY 
<YY 
IActionResultYY '
>YY' (!
GetTopProductsOverallYY) >
(YY> ?
)YY? @
{ZZ 	
try[[ 
{\\ 
var]] 
products]] 
=]] 
await]] $
_orderService]]% 2
.]]2 3&
GetTopProductsOverallAsync]]3 M
(]]M N
$num]]N P
)]]P Q
;]]Q R
return^^ 
Ok^^ 
(^^ 
products^^ "
)^^" #
;^^# $
}__ 
catch`` 
(`` 
	Exception`` 
ex`` 
)``  
{aa 
_loggerbb 
.bb 
LogErrorbb  
(bb  !
$"bb! #
$strbb# H
{bbH I
exbbI K
.bbK L
MessagebbL S
}bbS T
"bbT U
,bbU V
exbbW Y
)bbY Z
;bbZ [
returncc 

BadRequestcc !
(cc! "
newcc" %
{cc& '
Messagecc( /
=cc0 1
$strcc2 ;
+cc< =
excc> @
.cc@ A
MessageccA H
}ccI J
)ccJ K
;ccK L
}dd 
}ee 	
privatehh 
asynchh 
Taskhh 
<hh 
UserProfilehh &
>hh& '#
GetUserProfileFromTokenhh( ?
(hh? @
stringhh@ F
tokenhhG L
)hhL M
{ii 	
varjj 
tokenHandlerjj 
=jj 
newjj "#
JwtSecurityTokenHandlerjj# :
(jj: ;
)jj; <
;jj< =
trykk 
{ll 
varmm 
jwtTokenmm 
=mm 
tokenHandlermm +
.mm+ ,
	ReadTokenmm, 5
(mm5 6
tokenmm6 ;
)mm; <
asmm= ?
JwtSecurityTokenmm@ P
;mmP Q
varnn 
userIdnn 
=nn 
jwtTokennn %
?nn% &
.nn& '
Claimsnn' -
.nn- .
FirstOrDefaultnn. <
(nn< =
cnn= >
=>nn? A
cnnB C
.nnC D
TypennD H
==nnI K
$strnnL Q
)nnQ R
?nnR S
.nnS T
ValuennT Y
;nnY Z
ifpp 
(pp 
userIdpp 
==pp 
nullpp "
)pp" #
{qq 
returnrr 
nullrr 
;rr  
}ss 
returnuu 
newuu 
UserProfileuu &
{vv 
UserIdww 
=ww 
userIdww #
}xx 
;xx 
}yy 
catchzz 
(zz 
	Exceptionzz 
)zz 
{{{ 
return|| 
null|| 
;|| 
}}} 
}~~ 	
} 
}ÄÄ ˛.
[C:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Controllers\CartController.cs
[		 
ApiController		 
]		 
[

 
Route

 
(

 
$str

 
)

 
]

 
public 
class 
CartController 
: 
ControllerBase ,
{ 
private 
readonly 
CartService  
_cartService! -
;- .
public 

CartController 
( 
CartService %
cartService& 1
)1 2
{ 
_cartService 
= 
cartService "
;" #
} 
[ 
HttpGet 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $
GetCartItems% 1
(1 2
int2 5
userId6 <
)< =
{ 
try 
{ 	
var 
	cartItems 
= 
await !
_cartService" .
.. /%
GetCartItemsByUserIdAsync/ H
(H I
userIdI O
)O P
;P Q
if 
( 
	cartItems 
== 
null !
||" $
	cartItems% .
.. /
Count/ 4
==5 7
$num8 9
)9 :
{ 
return 
Ok 
( 
new 
List "
<" #
CartItemDto# .
>. /
(/ 0
)0 1
)1 2
;2 3
} 
return!! 
Ok!! 
(!! 
	cartItems!! 
)!!  
;!!  !
}"" 	
catch## 
(## 
	Exception## 
ex## 
)## 
{$$ 	
return%% 

BadRequest%% 
(%% 
$"%%  
$str%%  '
{%%' (
ex%%( *
.%%* +
Message%%+ 2
}%%2 3
"%%3 4
)%%4 5
;%%5 6
}&& 	
}'' 
[** 
HttpPost** 
(** 
$str** 
)** 
]** 
public++ 

async++ 
Task++ 
<++ 
IActionResult++ #
>++# $
	AddToCart++% .
(++. /
[++/ 0
FromBody++0 8
]++8 9
AddToCartRequest++: J
request++K R
)++R S
{,, 
if-- 

(-- 
request-- 
==-- 
null-- 
||-- 
request-- &
.--& '
Quantity--' /
<=--0 2
$num--3 4
)--4 5
{.. 	
return// 

BadRequest// 
(// 
$str// 0
)//0 1
;//1 2
}00 	
try22 
{33 	
await44 
_cartService44 
.44 
AddToCartAsync44 -
(44- .
request44. 5
.445 6
	ProductId446 ?
,44? @
request44A H
.44H I
UserId44I O
,44O P
request44Q X
.44X Y
Quantity44Y a
)44a b
;44b c
return55 
Ok55 
(55 
$str55 +
)55+ ,
;55, -
}66 	
catch77 
(77 %
InvalidOperationException77 (
ex77) +
)77+ ,
{88 	
return99 

BadRequest99 
(99 
ex99  
.99  !
Message99! (
)99( )
;99) *
}:: 	
};; 
[>> 
HttpPut>> 
(>> 
$str>> 
)>>  
]>>  !
public?? 

async?? 
Task?? 
<?? 
IActionResult?? #
>??# $
UpdateCartItem??% 3
(??3 4
[??4 5
FromBody??5 =
]??= >
UpdateCartRequest??? P
request??Q X
)??X Y
{@@ 
tryAA 
{BB 	
awaitCC 
_cartServiceCC 
.CC 
UpdateCartItemAsyncCC 2
(CC2 3
requestCC3 :
.CC: ;
	ProductIdCC; D
,CCD E
requestCCF M
.CCM N
UserIdCCN T
,CCT U
requestCCV ]
.CC] ^
QuantityCC^ f
)CCf g
;CCg h
returnDD 
OkDD 
(DD 
$strDD *
)DD* +
;DD+ ,
}EE 	
catchFF 
(FF %
InvalidOperationExceptionFF (
exFF) +
)FF+ ,
{GG 	
returnHH 

BadRequestHH 
(HH 
exHH  
.HH  !
MessageHH! (
)HH( )
;HH) *
}II 	
}JJ 
[MM 

HttpDeleteMM 
(MM 
$strMM "
)MM" #
]MM# $
publicNN 

asyncNN 
TaskNN 
<NN 
IActionResultNN #
>NN# $
RemoveCartItemNN% 3
(NN3 4
[NN4 5
	FromQueryNN5 >
]NN> ?
intNN@ C
	productIdNND M
,NNM N
[NNO P
	FromQueryNNP Y
]NNY Z
intNN[ ^
?NN^ _
userIdNN` f
)NNf g
{OO 
tryPP 
{QQ 	
awaitRR 
_cartServiceRR 
.RR 
RemoveCartItemAsyncRR 2
(RR2 3
	productIdRR3 <
,RR< =
userIdRR> D
)RRD E
;RRE F
returnSS 
OkSS 
(SS 
$strSS 7
)SS7 8
;SS8 9
}TT 	
catchUU 
(UU %
InvalidOperationExceptionUU (
exUU) +
)UU+ ,
{VV 	
returnWW 

BadRequestWW 
(WW 
exWW  
.WW  !
MessageWW! (
)WW( )
;WW) *
}XX 	
}YY 
}ZZ ﬂ
_C:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Controllers\CategoryController.cs
	namespace 	
MadkassenRestAPI
 
. 
Controllers &
{ 
[		 
Route		 

(		
 
$str		 
)		 
]		 
[

 
ApiController

 
]

 
public 

class 
CategoryController #
:$ %
ControllerBase& 4
{ 
private 
readonly  
ApplicationDbContext -
_context. 6
;6 7
public 
CategoryController !
(! " 
ApplicationDbContext" 6
context7 >
)> ?
{ 	
_context 
= 
context 
; 
} 	
[ 	
HttpGet	 
] 
public 
async 
Task 
< 
ActionResult &
<& '
IEnumerable' 2
<2 3
Kategori3 ;
>; <
>< =
>= >
GetAllCategories? O
(O P
)P Q
{ 	
return 
await 
_context !
.! "
Kategori" *
.* +
ToListAsync+ 6
(6 7
)7 8
;8 9
} 	
[ 	
HttpGet	 
( 
$str 
) 
] 
public 
async 
Task 
< 
ActionResult &
<& '
Kategori' /
>/ 0
>0 1
GetCategory2 =
(= >
int> A
idB D
)D E
{ 	
var 
category 
= 
await  
_context! )
.) *
Kategori* 2
.2 3
	FindAsync3 <
(< =
id= ?
)? @
;@ A
if!! 
(!! 
category!! 
==!! 
null!!  
)!!  !
{"" 
return## 
NotFound## 
(##  
)##  !
;##! "
}$$ 
return&& 
category&& 
;&& 
}'' 	
[** 	
HttpPost**	 
]** 
public++ 
async++ 
Task++ 
<++ 
ActionResult++ &
<++& '
Kategori++' /
>++/ 0
>++0 1
PostCategory++2 >
(++> ?
Kategori++? G
category++H P
)++P Q
{,, 	
_context-- 
.-- 
Kategori-- 
.-- 
Add-- !
(--! "
category--" *
)--* +
;--+ ,
await.. 
_context.. 
... 
SaveChangesAsync.. +
(..+ ,
).., -
;..- .
return00 
CreatedAtAction00 "
(00" #
nameof00# )
(00) *
GetCategory00* 5
)005 6
,006 7
new008 ;
{00< =
id00> @
=00A B
category00C K
.00K L

CategoryId00L V
}00W X
,00X Y
category00Z b
)00b c
;00c d
}11 	
}22 
}33 ∆'
[C:\Users\harun\soft-1\test-soft\backend-man2\MadkassenRestAPI\Controllers\AuthController.cs
	namespace 	
MadkassenRestAPI
 
. 
Controllers &
{		 
[

 
Route

 

(


 
$str

 
)

 
]

 
[ 
ApiController 
] 
public 

class 
AuthController 
(  
IUserService  ,
userService- 8
,8 9
IConfiguration: H
configurationI V
)V W
:X Y
ControllerBaseZ h
{ 
[ 	
HttpPost	 
] 
[ 	 
ProducesResponseType	 
( 
StatusCodes )
.) *
Status200OK* 5
)5 6
]6 7
[ 	 
ProducesResponseType	 
( 
StatusCodes )
.) *
Status400BadRequest* =
)= >
]> ?
public 
ActionResult 
< 
string "
>" #
Login$ )
() *
	AuthInput* 3
input4 9
)9 :
{ 	
try 
{ 
var 
user 
= 
userService &
.& '
Authenticate' 3
(3 4
input4 9
.9 :
Email: ?
,? @
inputA F
.F G
PasswordG O
)O P
;P Q
if 
( 
user 
== 
null  
)  !
{ 
return 
Unauthorized '
(' (
new( +
{, -
Message. 5
=6 7
$str8 V
}W X
)X Y
;Y Z
} 
var 
roles 
= 
user  
.  !
Roles! &
.& '
Split' ,
(, -
$char- 0
)0 1
;1 2
var 
token 
= 

JwtBuilder &
.& '
Create' -
(- .
). /
. 
WithAlgorithm "
(" #
new# &
HMACSHA256Algorithm' :
(: ;
); <
)< =
.   

WithSecret   
(    
configuration    -
[  - .
$str  . A
]  A B
)  B C
.!! 
Subject!! 
(!! 
user!! !
.!!! "
UserId!!" (
.!!( )
ToString!!) 1
(!!1 2
)!!2 3
)!!3 4
."" 
Issuer"" 
("" 
configuration"" )
["") *
$str""* >
]""> ?
)""? @
.## 
Audience## 
(## 
configuration## +
[##+ ,
$str##, B
]##B C
)##C D
.$$ 
IssuedAt$$ 
($$ 
DateTimeOffset$$ ,
.$$, -
Now$$- 0
.$$0 1
DateTime$$1 9
)$$9 :
.%% 
ExpirationTime%% #
(%%# $
DateTimeOffset%%$ 2
.%%2 3
Now%%3 6
.%%6 7
AddHours%%7 ?
(%%? @
$num%%@ A
)%%A B
.%%B C
DateTime%%C K
)%%K L
.&& 
	NotBefore&& 
(&& 
DateTimeOffset&& -
.&&- .
Now&&. 1
.&&1 2
DateTime&&2 :
)&&: ;
.'' 
AddClaim'' 
('' 
$str'' %
,''% &
roles''' ,
)'', -
.)) 
Id)) 
()) 
Guid)) 
.)) 
NewGuid)) $
())$ %
)))% &
.))& '
ToString))' /
())/ 0
)))0 1
)))1 2
.** 
Encode** 
(** 
)** 
;** 
return,, 
Ok,, 
(,, 
new,, 
{,, 
Token,,  %
=,,& '
token,,( -
},,. /
),,/ 0
;,,0 1
}-- 
catch.. 
(.. 
ArgumentException.. $
ex..% '
)..' (
{// 
return00 
Unauthorized00 #
(00# $
new00$ '
{00( )
Message00* 1
=002 3
ex004 6
.006 7
Message007 >
}00? @
)00@ A
;00A B
}11 
catch22 
(22 
	Exception22 
ex22 
)22  
{33 
return55 

StatusCode55 !
(55! "
StatusCodes55" -
.55- .(
Status500InternalServerError55. J
,55J K
new55L O
{55P Q
Message55R Y
=55Z [
$str	55\ é
,
55é è
Details
55ê ó
=
55ò ô
ex
55ö ú
.
55ú ù
Message
55ù §
}
55• ¶
)
55¶ ß
;
55ß ®
}66 
}77 	
}88 
}99 