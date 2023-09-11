mergeInto(LibraryManager.library, {
  webOpenJsonFile: function (callback) {
    let input = document.createElement("input");
    input.type = "file";
    input.setAttribute("accept", ".json");

    function logFile(event) {
      let str = event.target.result;

      let str1 = str;
      let len1 = lengthBytesUTF8(str1) + 1;
      let strPtr1 = _malloc(len1);
      stringToUTF8(str1, strPtr1, len1);
      Module.dynCall_vi(callback, strPtr1);
    }

    input.onchange = function (event) {
      if (!input.files.length) return;
      let reader = new FileReader();
      reader.onload = logFile;
      reader.readAsText(input.files[0]);
    };

    input.click();
  },
});
