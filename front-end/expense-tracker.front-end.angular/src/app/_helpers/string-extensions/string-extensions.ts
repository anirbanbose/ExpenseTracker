export { };

declare global {
  interface String {
    truncateWithEllipsis(maxLength: number): string;
  }
}

String.prototype.truncateWithEllipsis = function (maxLength: number): string {
  if (typeof this !== 'string' || maxLength <= 0) return '';
  let truncated = this.trim();
  if (truncated.length <= maxLength) return truncated;

  truncated = truncated.substring(0, maxLength + 1);
  const lastSpaceIndex = truncated.lastIndexOf(' ');
  if (lastSpaceIndex > 0) {
    truncated = truncated.substring(0, lastSpaceIndex);
  }

  return truncated + '…';
};