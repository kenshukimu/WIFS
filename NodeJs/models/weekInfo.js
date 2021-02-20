var Schema = {};
Schema.createSchema = function(mongoose) {
    var WeekinfoSchema = mongoose.Schema({
		year: {
            type:String,
            required:true
        },
        weekNo:{
            type:String,
            required:true
        },
        sdate:{
            type:String,
            required:true
        },
        edate:{
            type:String,
            required:true
        }
    }, {timestamps:true});

    mongoose.model('Weekinfo', WeekinfoSchema);
    console.log('WeekinfoSchema 정의함.');

    WeekinfoSchema.methods = {
		saveWeekInfo: function(callback) {
            var self = this;
            			
			this.validate(function(err) {
				if (err) return callback(err);
				
				self.save(callback);
			});
        }
    }    

    //스키마에 STATICS 메소드 추가
    WeekinfoSchema.statics = {
        findWeekInfo:function(param,callback) {            
            this.find(param)
            .then((result) => {
                callback(null,result);
            })
            .catch((err) => {
                console.log(err);
            });
        }
    }

	return WeekinfoSchema;
};

module.exports = Schema;