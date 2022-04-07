export const getCurrentDateOnly = () => {
    return getDateOnly(new Date().toISOString());
}

export const getDateOnly = (date: string) => {
	return date.toString().split('T')[0];
}

export const newsAccessDateComparison = (latestNewsDate: string, userLastAccessDate: string) => {
    let latestNewsDateArray: string[] = latestNewsDate.split('-');
    let userLastAccessDateArray: string[] = userLastAccessDate.split('-');

    //Check year
    if(latestNewsDateArray[0] > userLastAccessDateArray[0]) {
        return true;
    }
    //Check month
    if(latestNewsDateArray[1] > userLastAccessDateArray[1]) {
        return true;
    }
    //Check day
    if(latestNewsDateArray[2] > userLastAccessDateArray[2]) {
        return true;
    }

    return false;
}