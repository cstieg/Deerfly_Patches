function randomId(idLength, possibleChars) {
    possibleChars = possibleChars || 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890';
    var returnVal = '';
    for (var i = 0; i < idLength; i++) {
        returnVal += possibleChars.charAt(Math.floor((Math.random() * possibleChars.length)));
    }
    return returnVal;
}
