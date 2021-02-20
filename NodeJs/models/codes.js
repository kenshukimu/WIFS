var Schema = {};
Schema.createSchema = function(mongoose) {
    var CodesSchema = mongoose.Schema({
		codeGroup: {
            type:String,
            required:true
        },
        codeName:{
            type:String,
            required:true
        },
        codeValue:{
            type:String,
            required:true
        }
    }, {timestamps:true});

    mongoose.model('Codes', CodesSchema);
    console.log('CodesSchema 정의함.');

      // 스키마에 인스턴스 메소드 추가
      CodesSchema.methods = {
		saveCode: function(callback) {
            var self = this;
            			
			this.validate(function(err) {
				if (err) return callback(err);
				
				self.save(callback);
			});
        }
    }   

    //스키마에 STATICS 메소드 추가
    CodesSchema.statics = {
        findCode:function(param,callback) {            
            this.find(param)
            .then((result) => {
                callback(null,result);
            })
            .catch((err) => {
                console.log(err);
            });
        }
    }

	return CodesSchema;
};

module.exports = Schema;