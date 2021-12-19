/**
 * 배열 객체 안의 배열 요소가 가지는 인덱스 값 리턴
 */
const config = require('../config/config');
const jwt = require('jsonwebtoken');

exports.indexOf = function (arr, obj) {
	var index = -1;
	var keys = Object.keys(obj);

	var result = arr.filter(function (doc, idx) {
		var matched = 0;

		for (var i = keys.length - 1; i >= 0; i--) {
			if (doc[keys[i]] === obj[keys[i]]) {
				matched++;

				if (matched === keys.length) {
					index = idx;
					return idx;
				}
			}
		}
	});

	return index;
}

/**
 * 배열 안의 요소 중에서 파라미터와 같은 객체를 리턴
 */

exports.findByParam = function (arr, obj, callback) {
	var index = exports.indexof(arr, obj)
	if (~index && typeof callback === 'function') {
		return callback(undefined, arr[index])
	} else if (~index && !callback) {
		return arr[index]
	} else if (!~index && typeof callback === 'function') {
		return callback('not found')
	}
}

// 넘어온 값이 빈값인지 체크합니다. // !value 하면 생기는 논리적 오류를 제거하기 위해 // 명시적으로 value == 사용 // [], {} 도 빈값으로 처리 
exports.isEmpty = function(value){ 
		if( value == "" || 
			value == null || 
			value == undefined || 
			( value != null && typeof value == "object" && !Object.keys(value).length ) ){
				 return true }
	  else{ return false } };
/*
//토큰 유효성 검사 메소드
exports.verifyAccessToken = (req, res, next) => {
	var message='';

	//console.log(req.headers.authorization);
	//console.log(config.JWT_SECRET);
	 // 인증 완료	
	 try {
		// 요청 헤더에 저장된 토큰(req.headers.authorization)과 비밀키를 사용하여 토큰 반환
		req.decoded = jwt.verify(req.headers.authorization, config.JWT_SECRET);
		//console.log(req.decoded);
		return message;
	  }
	
	  // 인증 실패
	  catch (error) {
		// 유효기간이 초과된 경우
		if (error.name === 'TokenExpiredError') {	
			message = '419'	  
		//	message = '토큰이 만료되었습니다.'
		}else{
			message = '401'	  
		}	
		// 토큰의 비밀키가 일치하지 않는 경우
		//message = '유효하지 않은 토큰입니다.'
		
		return message;
	  }	  
  }
  exports.verifyRefreshToken = (req, res, next) => {
	var message='';

	//console.log(req.headers.authorization);
	//console.log(config.JWT_SECRET);
	 // 인증 완료
	 try {
		// 요청 헤더에 저장된 토큰(req.headers.authorization)과 비밀키를 사용하여 토큰 반환
		req.decoded = jwt.verify(req.headers.authorization, config.JWT_REFRESH);
		//console.log(req.decoded);
		return message;
	  }
	
	  // 인증 실패
	  catch (error) {
		// 유효기간이 초과된 경우
		if (error.name === 'TokenExpiredError') {	
			message = '419'	  
		//	message = '토큰이 만료되었습니다.'
		}else{
			message = '401'	  
		}	
		// 토큰의 비밀키가 일치하지 않는 경우
		//message = '유효하지 않은 토큰입니다.'
		
		return message;
	  }	  
  }*/