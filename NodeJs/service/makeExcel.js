/*
* TITLE  : 엑셀출력 모듈
* AUTHOR : 김현수
* DATE   : 2021.02.25
* DESC   : node-excel-export모듈을 이용하여 엑셀을 출력처리 할 수 있는 서비스
*/

//엑셀처리 모듈
const excel = require('node-excel-export');
//날짜 형식함수
const moment = require('moment');

var makeExcel = {};

//엑셀출력 기본모듈
makeExcel.makeExcelFile =
        function makeExcelFile(param, sheetName) 
        {
            const styles = {
                headerDark: {
                  fill: {
                    fgColor: {
                      rgb: 'FF000000'
                    }
                  },
                  font: {
                    color: {
                      rgb: 'FFFFFFFF'
                    },
                    sz: 14,
                    bold: true,
                    underline: true
                  }
                },
                cellPink: {
                  fill: {
                    fgColor: {
                      rgb: 'FFFFCCFF'
                    }
                  }
                },
                cellGreen: {
                  fill: {
                    fgColor: {
                      rgb: 'FF00FF00'
                    }
                  }
                }
              };
               
              //Array of objects representing heading rows (very top)
              const heading = [
                [{value: 'a1', style: styles.headerDark}, {value: 'b1', style: styles.headerDark}, {value: 'c1', style: styles.headerDark}],
                ['a2', 'b2', 'c2'] // <-- It can be only values
              ];

              //컬럼 헤더를 설정한다.
               let _specification = { 
                   /* SAMPLE
                     // 헤더 역할을 합니다. 
                     customer_name: { // <- the key should match the actual data key
                        displayName: 'Customer', // <- Here you specify the column header
                        headerStyle: styles.headerDark, // <- Header style
                        cellStyle: function(value, row) { // <- style renderer function
                          // if the status is 1 then color in green else color in red
                          // Notice how we use another cell value to style the current one
                          return (row.status_id == 1) ? styles.cellGreen : {fill: {fgColor: {rgb: 'FFFF0000'}}}; // <- Inline cell style is possible 
                        },
                        width: 120 // <- width in pixels
                      },
                      status_id: {
                        displayName: 'Status',
                        headerStyle: styles.headerDark,
                        cellFormat: function(value, row) { // <- Renderer function, you can access also any row.property
                          return (value == 1) ? 'Active' : 'Inactive';
                        },
                        width: '10' // <- width in chars (when the number is passed as string)
                      },
                      note: {
                        displayName: 'Description',
                        headerStyle: styles.headerDark,
                        cellStyle: styles.cellPink, // <- Cell style
                        width: 220 // <- width in pixels
                      }
                    */
                };
                /* SAMPLE DATA
                const dataset = { 
                    excelTitle:[
                        {columnKey : "customer_name" , title:'테스트1'},
                        {columnKey : "status_id" , title:'테스트2'},
                        {columnKey : "note" , title:'테스트3'},
                        {columnKey : "misc" , title:'테스트4'}
                    ],
                    data:[
                        {customer_name: 'IBM', status_id: 1, note: 'some note', misc: 'not shown'},
                        {customer_name: 'HP', status_id: 0, note: 'some note'},
                        {customer_name: 'MS', status_id: 0, note: 'some note', misc: 'not shown'}
                    ]
                };
                */

                //헤더를 동적으로 생성할 수 있도록 처리
                //헤더를 이중배열로 받아서 처리
                var _Datas = param.dataSet; 

                var _title = _Datas['excelTitle'];
                var _columnKey = new Array(_title.length);

                //헤더 설정
                for(let idx=0; idx<_title.length; idx++) { 
                    let tempKey = _title[idx]['columnKey']; 
                    let tempObj = new Object(); 
                    tempObj[tempKey] = new Object(); 
                    tempObj[tempKey]['displayName'] = _title[idx]['title']; 
                    tempObj[tempKey]['headerStyle'] = {}; 
                    tempObj[tempKey]['cellStyle'] = {}; 
                    tempObj[tempKey]['width'] = 120; 

                    _columnKey[idx] = tempKey;
                    Object.assign(_specification, tempObj) 
                }


                //상세표시 데이터
                var _data = _Datas['data'];

                //시트별데이터
                var _sheetData =new Array();
                //기본데이터
                var _excelData = new Array();
                                
                var key = '';
                //데이터 설정
                for(let idx=0; idx<_data.length; idx++) {    
                    if(idx != 0 && key != _data[idx]['id']) {
                      _sheetData.push(_excelData);
                      _excelData = new Array();
                    }

                    let tempObj = new Object();   
                    for(let i=0; i<_columnKey.length;i++) {
                        var _itemValue = typeof _data[idx][_columnKey[i]] == 'undefined'?'': _data[idx][_columnKey[i]];

                        //날짜 컬럼일 경우 Formatter이용하여 설정
                        if(_columnKey[i].indexOf('Date') > -1) {
                            _itemValue = moment(_itemValue).format("YYYY년MM월DD일");
                        }

                        if(_columnKey[i].indexOf('workTimeS') > -1) {
                            _itemValue = moment(_itemValue,'YYYYMMDDHHmmss').format("HH시mm분");
                        }

                        if(_columnKey[i].indexOf('workTimeE') > -1) {
                            _itemValue = moment(_itemValue,'YYYYMMDDHHmmss').format("HH시mm분");
                        }

                        tempObj[_columnKey[i]] = _itemValue;
                    }
                    key = _data[idx]['id'];
                    _excelData.push(tempObj); 
                }
                _sheetData.push(_excelData);
              
                let exportData = [ 
                   /*
                                   { 
                                        name: sheetName, 
                                        // 시트 이름
                                        specification: _specification, 
                                        // 위에서 만든 헤더
                                        data: _excelData
                                        // 시트에 들어가는 데이터
                                    } 
                                    // ,{ name: '두번채시트', specification: monthlySpecification, data: monthlyDataSet }
                    */
                                 ]; 

            for(let idx=0; idx<_sheetData.length; idx++) { 

                var sheetName = "";
                _sheetData[idx].forEach(element => {
                  sheetName = element.name + '_' + element.id;
                  //앞에 한번만 쓰면 됨..
                  return false;
                });  


                let tempObj = new Object();
                tempObj.name = sheetName; 
                tempObj.specification = _specification; 
                tempObj.data = _sheetData[idx]; 

                exportData.push(tempObj);
            }

            // exportData 역시 specification 처럼 for문을 통해 추가가능 
            let report = excel.buildExport(exportData); 
            return report 
        }

//엑셀 헤더 및 컬럼ID 설정
makeExcel.makeExcelFileHeader =
        function makeExcelFileHeader(headerList){
            var excelTitle = new Array(); 

            for (var i = 0; i < headerList.length; i++) {                
                var item = new Object();
                item.columnKey = headerList[i][0];
                item.title = headerList[i][1];
                excelTitle.push(item);        
            }
            return excelTitle;
        }

module.exports = makeExcel;