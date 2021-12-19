/*
DB실행 Shell
mongod --dbpath /Users/user/database/local
db.getCollection('users').find({})

*/
const express = require('express')
    , http = require('http')
    , path = require('path');

const bodyParser = require('body-parser')
    ,static = require('serve-static');

const mongoose = require('mongoose');

const session = require('express-session');

const database = require('./database/database');
const route_loader = require('./routes/route_loader');

const auth_middleware = require('./routes/auth.middleware');
const userController = require('./controllers/userController');

//설정파일
const config = require('./config/config');

//cors 설정 (API를 사용할 경우 CrossDomain에 걸릴 수 있으므로 설정할 수 있다.)
var cors = require('cors');

var whitelist = ['http://a.com', 'http://b.co.kr']

var corsOptions = {
  origin: function(origin, callback){
  var isWhitelisted = whitelist.indexOf(origin) !== -1;
  callback(null, isWhitelisted); 
  // callback expects two parameters: error and options 
  },
  credentials:true
}

var app = express();
app.use(cors(corsOptions));
app.all('*',function(req, res, next) {
    res.header("Access-Control-Allow-Origin", "*");
    res.header("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE");
    res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
    next();
});

//session설정
app.use(session({
    secret: 'hsookim!@#$',
    resave: false,
    saveUninitialized: true
   }));   

//===== 라우터 설정 =====
var router = express.Router();
//===== 뷰 엔진 설정 =====
app.set('view engine', 'ejs');
//app.set('view engine', 'html');
//app.engine('.html', require('ejs').__express);
app.set('views', __dirname + '/views');

// body-parser를 이용해 application/x-www-form-urlencoded 파싱
app.use(bodyParser.urlencoded({ extended: false }))

// body-parser를 이용해 application/json 파싱
app.use(bodyParser.json())
app.set('port', process.env.PORT || 3000);

//app.use(static('public'));
app.use(express.static(path.join(__dirname, 'public')));
app.use('/node_modules', express.static(path.join(__dirname,'/node_modules')));

var router = express.Router();

http.createServer(app).listen(app.get('port'), function() {
    console.log('ExpressServer  기동');
    database.init(app, config);
});

app.use('/API/:id' ,async function (req, res, next)  { 
    var _statusCode = auth_middleware.verifyToken(req, res)

    console.log("_statusCode1 : " + _statusCode);

    if(_statusCode != '200')  {
        _statusCode = await auth_middleware.verifyRefreshToken(req,res);   

        console.log("_statusCode2 : " + _statusCode);

        if(_statusCode != '401')  {
            //Access값을 재 전송
            //next();
            var access_token = userController.GetAccessToken(_statusCode);

            console.log("_access_token : " + access_token);

            var param = {
                accesstoken:access_token
            };            
            res.send(param);
        }else{
            return res.status(401).json({status: false, message: "Refresh Token is not valid"});
        }
    }else{
        next();
    };    
});

//app설정이 다 된 후 처리
route_loader.init(app, router);

//토큰 채크를 위한 라우터
//router.post('/token', auth_middleware.verifyRefreshToken, userController.GetAccessToken);

//기본 서버 화면 설정
app.use('/',function(req, res, next) { 
    res.render('index' , {message:''});         
});

