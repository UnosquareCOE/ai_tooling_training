export const clearUnmaskedWord = (game: any) => {
    const withoutUnmasked = {
      ...game,
    };
    delete withoutUnmasked.unmaskedWord;
    return withoutUnmasked;
  };