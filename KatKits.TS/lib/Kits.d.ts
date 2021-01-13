export declare const UTCToLocalDateTime: (UTCDate: Date) => Date;
/**
 * formate datetime toString like C#(but not look like).
 * @param CurrentDate JS Date
 * @param Formate y��,M��,d��,H:ʱ(24),h:ʱ(12),m:��,s:��,z:ʱ��(GMT��NN:00),����Ӧ���ִӺ���ǰ�滻��Ӧ��ĸ���������ĸ��0���룬�����ַ�ԭ�����,ʱ����֧��һ����ĸz,ʹ��ʱ�������滻��ĸz��12Сʱֵ����h���ֺ�����' AM'�� PM��;
 */
export declare const FormateDate: (CurrentDate: Date, Formate: string) => string;
/**
 * NumberString Index
 * @param Str Source String
 * @param Index <0 : return 0;=length : return .;>length : return 0;
 */
export declare const IndexOfNumberString: (Str: string, Index: number) => string;
export declare const RemoveBreaks: (OldString: string) => string;
export declare const RemoveBreaksAndWriteSpaceBehind: (OldString: string) => string;
