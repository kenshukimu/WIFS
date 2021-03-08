/**
 * 배열 객체 안의 배열 요소가 가지는 인덱스 값 리턴
 */

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
