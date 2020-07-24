module.exports = function (content,callback)
{
    var jsreport = require('jsreport-core')();

    jsreport.init().then(function ()
    {
            return jsreport.render({
            template: {
                content: content,
                engine: 'jsrender',
                recipe: 'phantom-pdf'
            }
        }).then(function (resp) {
            callback(/* error */ null, resp.content.toJSON().data);
        });
    }).catch(function (e) {
        callback(/* error */ e, null);
    })
};