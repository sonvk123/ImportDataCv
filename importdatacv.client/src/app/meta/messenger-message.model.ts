// messenger-message.model.ts
export interface MessengerMessage {
  ConversationId: string;
  UpdatedTime?: Date;
  MessageId?: string;
  MessageCreatedTime?: Date;
  FromId?: string;
  FromName?: string;
  FromEmail?: string;
  ToId?: string;
  ToName?: string;
  ToEmail?: string;
  MessageText?: string;
}
