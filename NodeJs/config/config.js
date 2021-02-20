
/*
 * 설정 
	db_url : 'mongodb+srv://root:1234qwer!@hscluster.yyh81.mongodb.net/wifs?retryWrites=true&w=majority',
	db_url : 'mongodb://hsookim:1234qwer!@ec2-54-180-140-98.ap-northeast-2.compute.amazonaws.com:27017',
 */

module.exports = {
	server_port: 3000,
	db_url : 'mongodb://hsookim:1234qwer!@ec2-54-180-140-98.ap-northeast-2.compute.amazonaws.com:27017/wifs?authSource=admin',
	gmailID: 'kenshukimu@gmail.com',
	gmailPassword: '!rhdvkfvkf08',
	db_schemas: [
		{file:'../models/users', collection:'users', schemaName:'Users', modelName:'UserModel'},
		{file:'../models/codes', collection:'codes', schemaName:'Codes', modelName:'CodesModel'},
		{file:'../models/weekInfo', collection:'weekinfos', schemaName:'Weekinfo', modelName:'WeekinfoModel'},
		{file:'../models/workInfo', collection:'workinfos', schemaName:'Workinfo', modelName:'WorkinfoModel'}
	],
	route_info: [
		{file:'../controllers/userController', path:'/login', method:'user_login', type:'post'},
		{file:'../controllers/userController', path:'/logout', method:'user_logout', type:'post'},
		{file:'../controllers/userController', path:'/userAll', method:'user_all', type:'post'},
		{file:'../controllers/userController', path:'/userFind', method:'user_find', type:'post'},
		{file:'../controllers/userController', path:'/userAdd', method:'user_add', type:'post'},
		{file:'../controllers/userController', path:'/userDel', method:'user_delete', type:'post'},
		{file:'../controllers/userController', path:'/userUpdate', method:'user_update', type:'post'},
		{file:'../controllers/userController', path:'/userReg', method:'init_userReg', type:'get'},

		{file:'../controllers/workController', path:'/workInfo', method:'init_workInfo', type:'get'},
		{file:'../controllers/workController', path:'/workUsers', method:'init_workUsers', type:'get'},
		{file:'../controllers/workController', path:'/workVac', method:'init_workVac', type:'get'},

		{file:'../controllers/codeController', path:'/getCode', method:'code_Find', type:'post'},
		{file:'../controllers/codeController', path:'/getWeekInfo', method:'weekInfo_Find', type:'post'},

		{file:'../controllers/workController', path:'/workInfoAdd', method:'workinfo_add', type:'post'},
		{file:'../controllers/workController', path:'/workInfoFind', method:'workinfo_find', type:'post'},

		{file:'../controllers/userController', path:'/userWithWorkListForChart', method:'user_with_workList_forChart', type:'post'},
		{file:'../controllers/userController', path:'/userWithWorkList', method:'user_with_workList', type:'post'},

		{file:'../controllers/workController', path:'/popupWorkInfo', method:'move_popupWorkInfo', type:'get'},
		{file:'../controllers/workController', path:'/workInfoUpdate', method:'workInfo_update', type:'post'},
		{file:'../controllers/workController', path:'/workInfoDelete', method:'workInfo_delete', type:'post'},

		{file:'../controllers/dataController', path:'/dataExport', method:'init_export', type:'get'},
		{file:'../controllers/dataController', path:'/pdfExport', method:'prt_overTime_report', type:'post'},
		{file:'../controllers/dataController', path:'/savePdf', method:'savePdf', type:'get'}
	]
}


