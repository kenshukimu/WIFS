const jwt = require('jsonwebtoken');
const redis_client = require('../config/redis_connect');
const config = require('../config/config');

function verifyToken(req, res, next) {
    try {
        // Bearer tokenstring
        const token = req.headers.authorization;
        const decoded = jwt.verify(token, config.JWT_SECRET);
    } catch (error) {
        //return res.status(401).json({status: false, message: "Your session is not valid.", data: error});
        // 인증 실패
		// 유효기간이 초과된 경우
		if (error.name === 'TokenExpiredError') {	
             //return res.status(419).json({status: false, message: '토큰이 만료되었습니다.', data: error});
             return "419";

             //await verifyRefreshToken(req, res);
		}else{
			//return res.status(401).json({status: false, message: '유효하지 않은 토큰입니다.', data: error});
            return "401";
		}	
    }
    return "200";
}

async function verifyRefreshToken(req, res, next) {
   
    const token = req.body.token;

    if(token === null) return res.status(401).json({status: false, message: "Invalid request."});
    try {
        const decoded = jwt.verify(token, config.JWT_REFRESH);    
        //console.log("client : " + JSON.parse(await redis_client.get(decoded.id.toString())).token);
        //console.log("auth.middle1");
        if(JSON.parse(await redis_client.get(decoded.id.toString())).token != token) {

            //console.log("auth.middle2");
            //return res.status(401).json({status: false, message: "Invalid request. Token is not same in store."});
            return "401";
        }

        // verify if token is in store or not
        /*
        redis_client.get(decoded.id.toString(), (err, data) => {
            if(err) throw err;

            if(data === null) return res.status(401).json({status: false, message: "Invalid request. Token is not in store."});
            if(JSON.parse(data).token != token) return res.status(401).json({status: false, message: "Invalid request. Token is not same in store."});

            next();
        })
        */
       
        return decoded.id.toString();
    } catch (error) {
        //return res.status(401).json({status: true, message: "Your session is not valid.", data: error});
        return "401";
    }
}

module.exports = {
    verifyToken,
    verifyRefreshToken
}